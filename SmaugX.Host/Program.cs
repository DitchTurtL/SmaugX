using SmaugX.Core.Hosting;

await SystemInitializer.StartUp();

await Server.Start();

await SystemInitializer.ShutDown();
