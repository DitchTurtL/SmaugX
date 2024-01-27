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

        var files = Directory.GetFiles(FileConstants.SQL_PATH, "*.sql");

        // Sort alphabetically by filename so our testdata can go after table creation.
        files = files.OrderBy(x => Path.GetFileName(x)).ToArray();

        foreach (var file in files)
        {
            Log.Information("running script: {file}", file);
            var contents = File.ReadAllText(file);
            await databaseService.ExecuteScript(contents);

        }

        Log.Information("Database initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
