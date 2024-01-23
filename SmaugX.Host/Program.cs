///
/// Smaug X
///   A MUD Server.
/// 
/// Program.cs
///   Main appliation entry point.
///

using Serilog;
using Serilog.Sinks.File;
using SmaugX.Core.Hosting;
using SmaugX.Core.Constants;

// Verify log directory exists
var logPath = Path.Combine(Environment.CurrentDirectory, FileConstants.LOG_DIRECTORY);
if (!Directory.Exists(logPath))
    Directory.CreateDirectory(FileConstants.LOG_DIRECTORY);

// Start serilogger
var logFile = Path.Combine(logPath, FileConstants.LOG_FILE);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Start Core services
await SystemInitializer.StartUp();

// Start Server running
await Server.Start();

// Shutdown Core services
await SystemInitializer.ShutDown();


await Log.CloseAndFlushAsync();