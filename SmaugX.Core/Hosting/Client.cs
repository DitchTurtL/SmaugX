using Serilog;
using SmaugX.Core.Commands;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Helpers;
using SmaugX.Core.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SmaugX.Core.Hosting;

public class Client
{
    // Sockets
    private TcpClient Socket { get; set; }
    private NetworkStream? Stream { get; set; }
    private Queue<string> OutputQueue { get; set; } = new();

    // Authentication
    public string AuthenticatedEmailOrUsername { get; set; }
    internal User? AuthenticatedUser { get; set; } = null;
    internal AuthenticationState AuthenticationState { get; set; } = AuthenticationState.NotAuthenticated;
    
    // Character
    internal Character? Character { get; set; } = null;
    internal CharacterCreationState CharacterCreationState { get; set; } = CharacterCreationState.None;

    /// <summary>
    /// Returns IP Address of connected client socket.
    /// </summary>
    public string IpAddress => (Socket?.Client?.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "Unknown";

    public Client(TcpClient socket)
    {
        Socket = socket;
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
            Server.ClientExited(this);
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

        await AuthenticationService.StartAuthentication(this);
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
        await CommandService.HandleCommand(command);
    }

    #endregion

    #region Send Content to Client Helpers

    /// <summary>
    /// Sends the welcome banner to the client.
    /// </summary>
    private async Task SendBanner()
    {
        await SendLines(ContentService.Banner());
    }

    /// <summary>
    /// Sends the MOTD to the client.
    /// </summary>
    public async Task SendMotd()
    {
        await SendLines(ContentService.Motd());
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

    /// <summary>
    /// Sends a complete line of text to the client.
    /// Appends a NewLine if it does not exist.
    /// </summary>
    internal async Task SendLine(string text)
    {
        if (!text.EndsWith(Environment.NewLine))
            text += Environment.NewLine;

        await SendText(text);
    }

    internal async Task SendText(string text)
    {
        Log.Debug("Sending data - {ipAddress}: {line}", IpAddress, text);

        // Colorize the text before sending.
        text = Colors.Colorize(text) + Environment.NewLine;

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
    internal async Task SendLines(string[] bannerLines)
    {
        foreach (var line in bannerLines)
            await SendLine(line);
    }

    /// <summary>
    /// Returns all characters belonging to this user.
    /// </summary>
    internal async Task<IEnumerable<Character>> GetCharacters()
    {
        if (AuthenticatedUser == null)
            return new List<Character>();

        return await DatabaseService.GetCharactersByUserId(AuthenticatedUser.Id);
    }

    /// <summary>
    /// Returns a character belonging to thus User with the matching name.
    /// </summary>
    internal async Task<Character?> GetCharacterByName(string name)
    {
        if (AuthenticatedUser == null)
            return null;

        return await DatabaseService.GetCharacterByNameWithUserId(name, AuthenticatedUser!.Id);
    }

    internal async Task CharacterSelected(Character character)
    {
        Character = character;

        // Notify the game service that the character has joined.
        await GameService.CharacterJoined(this);
    }

    #endregion

}
