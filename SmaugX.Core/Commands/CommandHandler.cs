using Serilog;
using SmaugX.Core.Commands.Authentication;

namespace SmaugX.Core.Commands;

/// <summary>
/// Main Command Handling class.
/// This class is responsible for handling all commands.
/// It will either have the command handled by the appropriate handler, 
/// or handle the command itself.
/// </summary>
internal class CommandHandler : ICommandHandler
{
    private List<ICommandHandler> commandHandlers { get; } = new();

    /// <summary>
    /// Initializes all command handlers.
    /// </summary>
    public CommandHandler()
    {
        Log.Information("Initializing Command Handler...");
        commandHandlers.Add(new AuthenticationCommandHandler());
    }

    public async Task HandleCommand(ICommand command)
    {
        // We can have multiple command handlers
        // for different modules of the game.
        foreach (var handler in commandHandlers)
        {
            // Don't handle any commands if the client is not authenticated.
            if (command.Client.AuthenticationState != AuthenticationState.Authenticated)
            {
                // Except for authentication commands.
                if (handler is AuthenticationCommandHandler)
                    await handler.HandleCommand(command);
                
                // If the authentication handler handled the command, break out of the loop.
                if (command.Handled)
                    break;

                continue;
            }

            // If the command hasn't already been handled, handle it.
            if (!command.Handled)
                await handler.HandleCommand(command);

            // If the command has been handled, break out of the loop.
            if (command.Handled)
                break;
        }

        // If the command hasn't been handled, handle it ourselves.
        if (!command.Handled)
            await HandleBaseCommand(command);
    }

    /// <summary>
    /// Handles all of our base commands.
    /// </summary>
    private async Task HandleBaseCommand(ICommand command)
    {
        switch (command.Name)
        {
            case "HELP":
                //await HandleHelpCommand(command);
                break;
            case "EXIT":
                //await HandleExitCommand(command);
                break;
            default:
                await HandleUnknownCommand(command);
                break;
        }
    }

    /// <summary>
    /// Handles commands that are not handled by anything.
    /// Pretty much catches typos.
    /// </summary>
    private async Task HandleUnknownCommand(ICommand command)
    {
        Log.Warning("Unknown command - {ipAddress}: {command} parameters: {params}",
            command.Client.IpAddress, command.Name, string.Join(" ", command.Parameters));
        await Task.CompletedTask;
    }
}
