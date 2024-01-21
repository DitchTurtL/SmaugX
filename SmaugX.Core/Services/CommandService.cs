using Serilog;
using SmaugX.Core.Commands;

namespace SmaugX.Core.Services;

internal static class CommandService
{
    /// <summary>
    /// Holds our command handler.
    /// </summary>
    private static CommandHandler commandHandler { get; } = new();

    public static async Task Initialize()
    {
        Log.Information("Initializing Command Service...");
        await Task.CompletedTask;
    }

    public static async Task HandleCommand(ICommand command)
    {
        await commandHandler.HandleCommand(command);
    }
}
