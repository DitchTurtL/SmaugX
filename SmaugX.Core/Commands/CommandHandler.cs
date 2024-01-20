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


    public CommandHandler()
    {
        commandHandlers.Add(new AuthenticationCommandHandler());
    }

    public async Task HandleCommand(ICommand command)
    {
        foreach (var handler in commandHandlers)
        {
            await handler.HandleCommand(command);
            if (command.Handled)
                break;
        }

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
        Log.Warning("Unknown command - {ipAddress}: {command.Name} parameters: {params}",
            command.Client.IpAddress, command.Name, string.Join(" ", command.Parameters));
        await Task.CompletedTask;
    }
}
