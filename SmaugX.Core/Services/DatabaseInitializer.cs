using Dapper;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmaugX.Core.Constants;

namespace SmaugX.Core.Services;

public class DatabaseInitializer : IHostedService
{
    private readonly IDatabaseService databaseService;

    public DatabaseInitializer(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Initializing database...");

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        Log.Information("Database initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
