﻿using System.Net.Sockets;
using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Services;

namespace SmaugX.Core.Hosting;

public static class Server
{
    private static List<Client> clients = new();

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
                ClientConnected(client);
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
        clients.Add(client);

        // Send Welcome banner
        await SendBanner(client);

        // Send MOTD
        await SendMotd(client);
    }

    private static async Task SendBanner(Client client)
    {
        await client.SendLines(ContentService.Banner());
    }

    private static async Task SendMotd(Client client)
    {
        await client.SendLines(ContentService.Motd());
    }

    internal static void ClientExited(Client client)
    {
        Log.Information("Client Exited - {ipAddress}", client.IpAddress);
        clients.Remove(client);
    }
}
