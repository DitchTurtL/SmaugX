using Microsoft.Extensions.Hosting;
using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Hosting;
using System.Net.Sockets;

namespace SmaugX.Core.Services;

public class TcpServerService : IHostedService
{
    private readonly IGameService gameService;
    private readonly ICommandService commandService;
    private readonly IDatabaseService databaseService;

    public TcpServerService(IGameService gameService, ICommandService commandService, IDatabaseService databaseService)
    {
        this.gameService = gameService;
        this.commandService = commandService;
        this.databaseService = databaseService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tcpListener = new TcpListener(SystemConstants.IP_ADDRESS, SystemConstants.PORT);
        tcpListener.Start();
        Log.Information("Server listening at {ipAddress}:{port}", SystemConstants.IP_ADDRESS, SystemConstants.PORT);

        try
        {
            while (true)
            {
                var socket = await tcpListener.AcceptTcpClientAsync();
                var client = new Client(socket, gameService, commandService, databaseService);
                _ = client.HandleClientAsync();
                gameService.ClientConnected(client);
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Server exiting...");
        return Task.CompletedTask;
    }


}
