using System.Net.Sockets;
using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Services;

namespace SmaugX.Core.Hosting;

public static class Server
{
    private static List<Client> Clients = new();

    public static async Task Start()
    {
        var tcpListener = new TcpListener(SystemConstants.IP_ADDRESS, SystemConstants.PORT);
        tcpListener.Start();
        Log.Information("Server listening at {ipAddress}:{port}", SystemConstants.IP_ADDRESS, SystemConstants.PORT);

        try
        {
            while (true)
            {
                var socket = await tcpListener.AcceptTcpClientAsync();
                var client = new Client(socket);
                _ = client.HandleClientAsync();
                _ = ClientConnected(client);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error: {message}", ex.Message);
        }
        finally
        {
            Log.Information("Server exiting...");
        }
    }

    private static async Task ClientConnected(Client client)
    {
        Log.Information("Client connected - {ipAddress}", client.IpAddress);
        Clients.Add(client);
    }


    internal static void ClientExited(Client client)
    {
        Log.Information("Client Exited - {ipAddress}", client.IpAddress);
        Clients.Remove(client);
    }

    internal static async Task ClientAuthenticated(Client client)
    {
        Log.Information("Client Authenticated - {ipAddress} as {username}[{email}]", 
            client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown", 
            client.AuthenticatedUser?.Email ?? "Unknown");

        Clients.Remove(client);
        await GameService.UserJoined(client);
    }
}
