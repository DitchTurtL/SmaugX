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

namespace SmaugX.Core.Data.Hosting;

public class Client
{
    private readonly IGameService gameService;
    private readonly ICommandService commandService;
    private readonly IDatabaseService databaseService;

    // Sockets
    private TcpClient Socket { get; set; }
    private NetworkStream? Stream { get; set; }

    // Authentication
    public string AuthenticatedEmailOrUsername { get; set; }
    internal User? AuthenticatedUser { get; set; } = null;
    internal AuthenticationState AuthenticationState { get; set; } = AuthenticationState.NotAuthenticated;

    // Character
    private Character? character = null;
    internal Character Character
    {
        get => character ??= new Character();
        set => character = value;
    }

    internal CharacterCreationState CharacterCreationState { get; set; } = CharacterCreationState.None;

    /// <summary>
    /// Returns IP Address of connected client socket.
    /// </summary>
    public string IpAddress => (Socket?.Client?.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "Unknown";

    public Client(TcpClient socket, IGameService gameService, ICommandService commandService, IDatabaseService databaseService)
    {
        Socket = socket;
        this.gameService = gameService;
        this.commandService = commandService;
        this.databaseService = databaseService;
    }

    /// <summary>
    /// Handles the connected client's data exchange
    /// </summary>
    public async Task HandleClientAsync()
    {

        try
        {
            Stream = Socket.GetStream();
            await ClientConnected();

            var buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                await ReceivedData(receivedData);
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
            gameService.ClientExited(this);
        }
    }

    #region Events

    /// <summary>
    /// Called when the client first connects.
    /// </summary>
    private async Task ClientConnected()
    {

        // Send Welcome banner
        await SendBanner();

        StartAuthentication(this);
    }

    /// <summary>
    /// Called when data is received from the client.
    /// </summary>
    private async Task ReceivedData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;

        Log.Information("Received - {ipAddress}: {data}", IpAddress, data);
        var command = new Command(this, data);
        commandService.HandleCommand(command);
    }

    #endregion

    #region Send Content to Client Helpers

    /// <summary>
    /// Sends the welcome banner to the client.
    /// </summary>
    private async Task SendBanner()
    {
        await SendLines(ContentHelper.Banner(), MessageColor.Banner);
    }

    /// <summary>
    /// Sends the MOTD to the client.
    /// </summary>
    public async Task SendMotd()
    {
        await SendLines(ContentHelper.Motd(), MessageColor.Motd);
    }

    #endregion

    #region Send Data to Socket Helpers

    /// <summary>
    /// Sends binary data to the client socket.
    /// </summary>
    private async Task SendData(byte[] data)
    {
        await Stream!.WriteAsync(data, 0, data.Length);
    }

    internal async Task SendSystemMessage(string text)
    {
        await SendLine(text, MessageColor.System);
    }

    /// <summary>
    /// Sends a complete line of text to the client.
    /// Appends a NewLine if it does not exist.
    /// </summary>
    internal async Task SendLine(string text, MessageColor messageColor = MessageColor.None)
    {
        // Remove all line endings and new lines.
        text = text.ReplaceLineEndings().Replace(Environment.NewLine, string.Empty);

        // Colorize the text before sending.
        text = Colors.Colorize(text, messageColor);

        text += Environment.NewLine;

        await SendText(text, messageColor);
    }

    private async Task SendText(string text, MessageColor messageColor = MessageColor.None)
    {
        Log.Debug("Sending data - {ipAddress}: {line}", IpAddress, text);

        var bytes = Encoding.UTF8.GetBytes(text);

        try
        {
            await SendData(bytes);
        }
        catch (Exception ex)
        {
            Log.Error("Error sending data - {ipAddress} - {text} - {message}", IpAddress, text, ex.Message);
        }
    }

    /// <summary>
    /// Sends a number of lines to the client.
    /// </summary>
    internal async Task SendLines(string[] lines, MessageColor messageColor = MessageColor.None)
    {
        foreach (var line in lines)
            await SendLine(line, messageColor);
    }

    /// <summary>
    /// Returns all characters belonging to this user.
    /// </summary>
    internal async Task<IEnumerable<Character>> GetCharacters()
    {
        if (AuthenticatedUser == null)
            return new List<Character>();

        return databaseService.GetCharactersByUserId(AuthenticatedUser.Id);
    }

    /// <summary>
    /// Returns a character belonging to this User with the matching name.
    /// </summary>
    internal async Task<Character?> GetCharacterByName(string name)
    {
        if (AuthenticatedUser == null)
            return null;

        return databaseService.GetCharacterByIdAndName(AuthenticatedUser!.Id, name);
    }

    internal async Task CharacterSelected(Character character)
    {
        character.Client = this;
        Character = character;

        // Notify the game service that the character has joined.
        await gameService.CharacterJoined(this);
    }

    internal async Task SendSeparator()
    {
        await SendLine(StringConstants.SEPARATOR);
    }

    #endregion

    internal void StartCharacterCreation(Client client)
    {
        SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_START);
    }

    internal void StartAuthentication(Client client)
    {
        AuthenticationState = AuthenticationState.WaitingForEmail;
        SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_USERNAME);
    }
}
