///
/// Smaug X
///   A MUD Server.
/// 
/// Program.cs
///   Main appliation entry point.
///

using SmaugX.Core.Hosting;

// Start Core services
await SystemInitializer.StartUp();

// Start Server running
await Server.Start();

// Shutdown Core services
await SystemInitializer.ShutDown();
