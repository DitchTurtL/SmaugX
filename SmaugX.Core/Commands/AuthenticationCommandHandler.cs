﻿
using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Services;

namespace SmaugX.Core.Commands;

/// <summary>
/// Handles all authentication commands.
/// </summary>
internal class AuthenticationCommandHandler : ICommandHandler
{
    private readonly IGameService gameService;
    private readonly IDatabaseService databaseService;

    public AuthenticationCommandHandler(IGameService gameService, IDatabaseService databaseService)
    {
        this.gameService = gameService;
        this.databaseService = databaseService;
    }

    public void HandleCommand(ICommand command)
    {
        var commandTask = command.Client.AuthenticationState switch
        {
            AuthenticationState.WaitingForEmail => HandleEmail(command),
            AuthenticationState.WaitingForPassword => HandlePassword(command),
            _ => Task.CompletedTask,
        };
        commandTask.Wait();
    }

    private async Task HandleEmail(ICommand command)
    {
        // If nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            await command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // If the user supplied an email, save it and prompt for password.
        command.Client.AuthenticatedEmailOrUsername = command.Name;

        command.Client.AuthenticationState = AuthenticationState.WaitingForPassword;
        await command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_PASSWORD);
        command.Handled = true;
    }

    private async Task HandlePassword(ICommand command)
    {
        // If nothing was supplied, fail authentication and prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            await command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // If the user supplied a password, attempt to authenticate.
        var user = databaseService.GetUserForAuth(command.Client.AuthenticatedEmailOrUsername, command.Name);

        // If a user wasn't found, fail authentication and prompt again.
        if (user == null)
        {
            Log.Warning("Failed authentication for {emailOrUsername} from {ipAddress}", command.Client.AuthenticatedEmailOrUsername, command.Client.IpAddress);
            await command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // Otherwise, the user was found and authenticated.
        command.Client.AuthenticationState = AuthenticationState.Authenticated;
        command.Client.AuthenticatedUser = user;
        command.Handled = true;

        // Let the client know their authentication was successful
        await command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_SUCCESS);

        // Let the server know this client has authenticated.
        await gameService.ClientAuthenticated(command.Client);
    }

}
