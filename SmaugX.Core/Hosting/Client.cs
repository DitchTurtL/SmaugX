using Serilog;
using SmaugX.Core.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SmaugX.Core.Hosting;

public class Client
{
    private TcpClient Socket { get; set; }
    private NetworkStream? Stream { get; set; }

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
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Log.Information("Received: {receivedData}", receivedData);

                // Do Echo
                var responseBuffer = Encoding.UTF8.GetBytes(receivedData);
                await Stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
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

        // Send MOTD
        await SendMotd();
    }

    #endregion

    #region Send Content to Client Helpers

    private async Task SendBanner()
    {
        await SendLines(ContentService.Banner());
    }

    private async Task SendMotd()
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

        Log.Debug("Sending Line - {ipAddress}: {line}", IpAddress, text);
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

    internal async Task SendLines(string[] bannerLines)
    {
        foreach (var line in bannerLines)
            await SendLine(line);
    }

    #endregion

}
