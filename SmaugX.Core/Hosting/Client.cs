using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SmaugX.Core.Hosting;

public class Client
{
    private TcpClient Socket { get; set; }
    public string IpAddress => (Socket.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "Unknown";

    public Client(TcpClient socket)
    {
        Socket = socket;
    }

    public async Task HandleClientAsync()
    {
        try
        {
            var networkStream = Socket.GetStream();
            var buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Log.Information("Received: {receivedData}", receivedData);

                // Do Echo
                var responseBuffer = Encoding.UTF8.GetBytes(receivedData);
                await networkStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
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


}
