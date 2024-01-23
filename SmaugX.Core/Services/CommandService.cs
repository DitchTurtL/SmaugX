using Serilog;
using SmaugX.Core.Commands;
using SmaugX.Core.Commands.CommandHandlers;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;

namespace SmaugX.Core.Services;

public class CommandService : ICommandService, ICommandHandler
{
    private readonly IGameService gameService;
    private readonly IDatabaseService databaseService;

    private List<ICommandHandler> commandHandlers { get; }

    /// <summary>
    /// Initializes all command handlers with dependencies.
    /// </summary>
    public CommandService(IGameService gameService, IDatabaseService databaseService)
    {
        Log.Information("Initializing Command Handler...");
        this.gameService = gameService;
        this.databaseService = databaseService;

        commandHandlers = new()
        {
            new AuthenticationCommandHandler(gameService, databaseService),
            new CharacterCreationCommandHandler(),
        };   
    }

    /// <summary>
    /// This method will receive all commands from the client.
    /// It is responsible for handing the command off to other handlers.
    /// Then handling anything that has not been handled, itself.
    /// </summary>
    /// <param name="command"></param>
    public void HandleCommand(ICommand command)
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
                if (handler is CharacterCreationCommandHandler)
                    handler.HandleCommand(command);

                continue;
            }

            // If the command hasn't already been handled, handle it.
            if (!command.Handled)
                handler.HandleCommand(command);

            // If the command has been handled, break out of the loop.
            if (command.Handled)
                break;
        }

        // If the command hasn't been handled, handle it ourselves.
        if (!command.Handled)
            HandleBaseCommand(command);
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

    }
}
