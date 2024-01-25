using Serilog;
using SmaugX.Core.Commands;
using SmaugX.Core.Commands.CommandHandlers;
using SmaugX.Core.Commands.Commands;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;

namespace SmaugX.Core.Services;

public class CommandService : ICommandService, ICommandHandler
{
    private List<ICommandHandler> commandHandlers { get; } = new();
    private List<ICommand> commands { get; } = new();

    /// <summary>
    /// Initializes all command handlers with dependencies.
    /// </summary>
    public CommandService(IGameService gameService, IDatabaseService databaseService, IRoomService roomService)
    {
        Log.Information("Initializing Command Handler...");

        // Initialize command handlers.
        commandHandlers = new()
        {
            new AuthenticationHandler(gameService, databaseService),
            new CharacterCreationHandler(databaseService),
            new MovementHandler(roomService),
            new BuildHandler(roomService),
        };

        // Initialize commands
        commands = new()
        {
            new Build(),
        };
    }

    /// <summary>
    /// This method will receive all commands from the client.
    /// It is responsible for handing the command off to other handlers.
    /// Then handling anything that has not been handled, itself.
    /// </summary>
    public void HandleCommand(ICommand command)
    {
        try
        {
            // We can have multiple command handlers
            // for different modules of the game.
            foreach (var handler in commandHandlers)
            {
                // Don't handle any commands if the client is not authenticated.
                if (command.Client.AuthenticationState != AuthenticationState.Authenticated)
                {
                    // Except for authentication commands.
                    if (handler is AuthenticationHandler)
                        handler.HandleCommand(command);

                    // If the authentication handler handled the command, break out of the loop.
                    if (command.Handled)
                        break;

                    continue;
                }

                // Require character creation or population before being able to interact with the world.
                if (command.Client.CharacterCreationState != CharacterCreationState.Loaded)
                {
                    // Only allow character creation commands for now.
                    if (handler is CharacterCreationHandler)
                        handler.HandleCommand(command);

                    if (command.Handled)
                        break;

                    continue;
                }

                // If the command hasn't already been handled, handle it.
                if (!command.Handled)
                    handler.HandleCommand(command);

                // If the command has been handled, break out of the loop.
                if (command.Handled)
                    break;
            }

            // Try to handle the command as a modular command
            if (!command.Handled)
                HandleModularCommand(command);

            // If the command hasn't been handled, handle it ourselves.
            if (!command.Handled)
                HandleBaseCommand(command);
        } 
                catch (Exception ex)
        {
            Log.Error(ex, "Error handling command - {ipAddress}: {command} parameters: {params}",
                               command.Client.IpAddress, command.Name, string.Join(" ", command.Parameters));
        }
        
    }

    private void HandleModularCommand(ICommand command)
    {
        // Find the command template to clone.
        var cmd = commands.FirstOrDefault(x => x.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
        x.Name.Equals(command.Name + "Command"));

        if (cmd == null)
            return;
        
        var args = command.Parameters.ToList().Skip(1);

        // If there are no parameters, or the first parameter is HELP, show the help for the command.
        if (!args.Any() || args.First().Equals("HELP", StringComparison.OrdinalIgnoreCase))
        {
            var helpLines = cmd.GetHelp(args.ToArray());
            command.Client.SendLines(helpLines);
            command.Handled = true;
            return;
        }

        // If we got more than one parameter and it wasn't a help command,
        // we need to clone the command and run it in context for this user.

        // Clone the command template.
        var commandToRun = (ICommand)cmd.Clone();
        // Copy the parameters from the original command.
        commandToRun.Parameters = command.Parameters[1..];
        commandToRun.Client = command.Client;

        _ = commandToRun?.Run();
        command.Handled = true;
    }

    /// <summary>
    /// Handles all of our base commands.
    /// </summary>
    private void HandleBaseCommand(ICommand command)
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
                HandleUnknownCommand(command);
                break;
        }
    }

    /// <summary>
    /// Handles commands that are not handled by anything.
    /// Pretty much catches typos.
    /// </summary>
    private void HandleUnknownCommand(ICommand command)
    {
        Log.Warning("Unknown command - {ipAddress}: {command} parameters: {params}",
            command.Client.IpAddress, command.Name, string.Join(" ", command.Parameters));

        command.Client.SendSystemMessage(StringConstants.UNKNOWN_COMMAND);

    }
}
