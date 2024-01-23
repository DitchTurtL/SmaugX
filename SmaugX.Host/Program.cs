///
/// Smaug X
///   A MUD Server.
/// 
/// Program.cs
///   Main appliation entry point.
///

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Services;


// Verify log directory exists
var logPath = Path.Combine(Environment.CurrentDirectory, FileConstants.LOG_DIRECTORY);
if (!Directory.Exists(logPath))
    Directory.CreateDirectory(FileConstants.LOG_DIRECTORY);

// Start logger
var logFile = Path.Combine(logPath, FileConstants.LOG_FILE);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Configure services
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<TcpServerService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IRoomService, RoomService>();
        services.AddSingleton<ICommandService, CommandService>();
    })
    .Build();


await host.RunAsync();

// Flush logs
await Log.CloseAndFlushAsync();
