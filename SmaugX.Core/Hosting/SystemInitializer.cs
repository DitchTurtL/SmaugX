using Serilog;
using SmaugX.Core.Constants;

namespace SmaugX.Core.Hosting;

public class SystemInitializer
{
    public static async Task StartUp()
    {
        Log.Information("Starting up...");

        StartLogging();
    }

    public static async Task ShutDown()
    {
        Log.Information("Shutting down...");

        await Log.CloseAndFlushAsync();
    }

    private static void StartLogging()
    {
        // Verify log directory exists
        var logPath = Path.Combine(Environment.CurrentDirectory, FileConstants.LOG_DIRECTORY);
        Log.Debug("Verifying log directory exists - {logPath}", logPath);
        if (!Directory.Exists(logPath))
            Directory.CreateDirectory(FileConstants.LOG_DIRECTORY);

        // Start serilogger
        var logFile = Path.Combine(logPath, FileConstants.LOG_FILE);
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}
