using Serilog;
using SmaugX.Core.Commands;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Helpers;
using SmaugX.Core.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace SmaugX.Core.Data.Hosting;

public class Client
{
    #region Service References
    private readonly IGameService gameService;
    private readonly ICommandService commandService;
    private readonly IDatabaseService databaseService;
    private readonly TcpServerService serverService;
    #endregion

    // Send Queue to send messages to the client.
    private readonly Channel<string> sendQueue = Channel.CreateUnbounded<string>();

    // Sockets
    private TcpClient Socket { get; set; }
    private NetworkStream? Stream { get; set; }

    // Authentication
    internal User? AuthenticatedUser { get; set; } = null;
    internal AuthenticationState AuthenticationState { get; set; } = AuthenticationState.NotAuthenticated;

    // Character
    internal Character? Character { get; set; }
    internal CharacterCreationState CharacterCreationState { get; set; } = CharacterCreationState.None;

    /// <summary>
    /// Returns IP Address of connected client socket.
    /// </summary>
    public string IpAddress => (Socket?.Client?.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "Unknown";

    /// <summary>
    /// Initializer
    /// </summary>
    public Client(TcpClient socket, TcpServerService serverService, IGameService gameService, ICommandService commandService, IDatabaseService databaseService)
    {
        Socket = socket;
        this.gameService = gameService;
        this.commandService = commandService;
        this.databaseService = databaseService;
        this.serverService = serverService;
    }

    /// <summary>
    /// Handles the connected client's data exchange
    /// </summary>
    public async Task HandleClientAsync()
    {
        try
        {
            Stream = Socket.GetStream();
            _ = ConsumeMessagesAsync();
            ClientConnected();

            var buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                ReceivedData(receivedData);
            }

            Log.Information("Client disconnected - {ipAddress}", IpAddress);
        }
        catch (Exception ex)
        {
            Log.Error("Client Error: {ipAddress} - {message}", IpAddress, ex.Message);
        }
        finally
        {
            Socket.Close();
            serverService.ClientExited(this);
        }
    }

    #region Events

    /// <summary>
    /// Called when the client first connects.
    /// </summary>
    private void ClientConnected()
    {

        // Send Welcome banner
        SendBanner();

        StartAuthentication(this);
    }

    /// <summary>
    /// Called when data is received from the client.
    /// </summary>
    private void ReceivedData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;

        Log.Debug("Received - {ipAddress}: {data}", IpAddress, data);
        var command = new Command(this, data);
        commandService.HandleCommand(command);
    }

    /// <summary>
    /// Called when the client has selected a character.
    /// </summary>
    /// <param name="character"></param>
    internal void CharacterSelected(Character character)
    {
        character.Client = this;
        Character = character;

        // Notify the game service that the character has joined.
        gameService.CharacterJoined(this.Character);
    }

    #endregion

    #region Triggers

    /// <summary>
    /// Triggers the Character Creation process
    /// </summary>
    internal void StartCharacterCreation(Client client)
    {
        SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_START.Split(Environment.NewLine));
    }

    /// <summary>
    /// Triggers the Authentication process
    /// </summary>
    internal void StartAuthentication(Client client)
    {
        AuthenticationState = AuthenticationState.WaitingForEmail;
        SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_USERNAME);
    }

    #endregion

    #region Content Helpers

    /// <summary>
    /// Sends the welcome banner to the client.
    /// </summary>
    private void SendBanner()
    {
        SendLines(ContentHelper.Banner(), MessageColor.Banner);
    }

    /// <summary>
    /// Sends the MOTD to the client.
    /// </summary>
    public void SendMotd()
    {
        SendLines(ContentHelper.Motd(), MessageColor.Motd);
    }

    #endregion

    #region Send Message Helpers

    /// <summary>
    /// Sends a line separator to the client.
    /// </summary>
    internal void SendSeparator()
    {
        SendLine(StringConstants.SEPARATOR);
    }

    /// <summary>
    /// Sends a message to the client with default SystemMessage color.
    /// </summary>
    /// <param name="text"></param>
    internal void SendSystemMessage(string text)
    {
        SendLine(text, MessageColor.System);
    }

    internal void SendSystemMessage(string[] lines)
    {
        foreach (var line in lines)
            SendSystemMessage(line);
    }

    /// <summary>
    /// Sends a number of lines to the client.
    /// </summary>
    internal void SendLines(string[] lines, MessageColor messageColor = MessageColor.None)
    {
        foreach (var line in lines)
            SendLine(line, messageColor);
    }

    /// <summary>
    /// Sends a complete line of text to the client.
    /// Appends a NewLine if it does not exist.
    /// </summary>
    internal void SendLine(string text, MessageColor messageColor = MessageColor.None)
    {
        // Remove all line endings and new lines.
        text = text.ReplaceLineEndings().Replace(Environment.NewLine, string.Empty);

        // Colorize the text before sending.
        text = Colors.Colorize(text, messageColor);

        // Append the newline.
        text += Environment.NewLine;

        EnqueueMessage(text, messageColor);
    }

    /// <summary>
    /// Runs in background and consumes messages from the send queue.
    /// </summary>
    private async Task ConsumeMessagesAsync()
    {
        var reader = sendQueue.Reader;

        // Asynchronously iterate over the messages in the channel and send them
        await foreach (string message in reader.ReadAllAsync())
        {
            SendText(message);
        }
    }

    /// <summary>
    /// Enqueues a message to be sent to the client.
    /// </summary>
    private void EnqueueMessage(string message, MessageColor messageColor = MessageColor.None)
    {
        sendQueue.Writer.TryWrite(message);
    }

    /// <summary>
    /// Receives a string from the Message Queue Consumer and converts it to binary data to be sent to the client.
    /// </summary>
    private void SendText(string text, MessageColor messageColor = MessageColor.None)
    {
        Log.Debug("Sending data - {ipAddress}: {line}", IpAddress, text);

        var bytes = Encoding.UTF8.GetBytes(text);

        try
        {
            Task.Run(async () => await SendData(bytes)).Wait();
        }
        catch (Exception ex)
        {
            Log.Error("Error sending data - {ipAddress} - {text} - {message}", IpAddress, text, ex.Message);
        }
    }

    /// <summary>
    /// Sends binary data to the client socket.
    /// </summary>
    private async Task SendData(byte[] data)
    {
        await Stream!.WriteAsync(data, 0, data.Length);
    }

    internal void SendSystemMessage(object aUTHENTICATION_PROMPT_PASSWORD_CONFIRMATION)
    {
        throw new NotImplementedException();
    }

    #endregion
}
