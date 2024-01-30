using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.CommandHandlers;

/// <summary>
/// Handles all authentication commands.
/// </summary>
internal class AuthenticationHandler : ICommandHandler
{
    private readonly IGameService gameService;
    private readonly IDatabaseService databaseService;

    private Dictionary<Client, string> userOrEmailForAuth = new();
    private Dictionary<Client, string> passwordForAuth = new();

    public AuthenticationHandler(IGameService gameService, IDatabaseService databaseService)
    {
        this.gameService = gameService;
        this.databaseService = databaseService;
    }

    public void HandleCommand(ICommand command)
    {
        switch (command.Client.AuthenticationState)
        {
            case AuthenticationState.WaitingForEmail:
                if (command.Name.ToUpper() == "NEW")
                {
                    command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_NEW_USERNAME);
                    command.Client.AuthenticationState = AuthenticationState.CreatingNewUser;
                    command.Handled = true;
                    return;
                }

                HandleEmail(command);
                break;
            case AuthenticationState.WaitingForPassword:
                HandlePassword(command);
                break;
            case AuthenticationState.CreatingNewUser:
                HandleNewUser(command);
                break;
            case AuthenticationState.CreatingNewPassword:
                HandleNewPassword(command);
                break;
            case AuthenticationState.CreatingNewPasswordConfirmation:
                HandleNewPasswordConfirmation(command);
                break;
        }
    }

    private void HandleNewUser(ICommand command)
    {
        // If nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_NEW_USERNAME);
            command.Handled = true;
            return;
        }

        // Check if the user already exists.
        var tmpUser = databaseService.GetUserForAuth(command.Name, null);
        if (tmpUser != null)
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_USER_ALREADY_EXISTS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // If the user supplied a username, save it and prompt for password.
        userOrEmailForAuth[command.Client] = command.Name;

        command.Client.AuthenticationState = AuthenticationState.CreatingNewPassword;
        command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_PASSWORD);
        command.Handled = true;
    }

    private void HandleNewPassword(ICommand command)
    {
        // If nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_PASSWORD);
            command.Handled = true;
            return;
        }

        // If the user supplied a password, save it and prompt for confirmation.
        passwordForAuth[command.Client] = command.Name;

        command.Client.AuthenticationState = AuthenticationState.CreatingNewPasswordConfirmation;
        command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_PASSWORD_CONFIRMATION);
        command.Handled = true;
    }

    private void HandleNewPasswordConfirmation(ICommand command)
    {
        // If nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_PASSWORD_CONFIRMATION);
            command.Handled = true;
            return;
        }

        // Verify the password matches the confirmation.
        if (passwordForAuth[command.Client] != command.Name)
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // Create the user
        var user = databaseService.CreateUser(userOrEmailForAuth[command.Client], passwordForAuth[command.Client]);

        if (user == null)
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_FAILED_CREATE_USER);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        HandlePassword(command);
    }


    private void HandleEmail(ICommand command)
    {
        // If nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // If the user supplied an email, save it and prompt for password.
        userOrEmailForAuth[command.Client] = command.Name;

        command.Client.AuthenticationState = AuthenticationState.WaitingForPassword;
        command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_PASSWORD);
        command.Handled = true;
    }

    private void HandlePassword(ICommand command)
    {
        // If nothing was supplied, fail authentication and prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // If the user supplied a password, attempt to authenticate.
        userOrEmailForAuth.TryGetValue(command.Client, out var emailOrUsername);
        var user = databaseService.GetUserForAuth(emailOrUsername, command.Name);

        // If a user wasn't found, fail authentication and prompt again.
        if (user == null)
        {
            Log.Warning("Failed authentication for {emailOrUsername} from {ipAddress}", emailOrUsername, command.Client.IpAddress);
            command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_INVALID_CREDENTIALS);
            command.Client.StartAuthentication(command.Client);
            command.Handled = true;
            return;
        }

        // Otherwise, the user was found and authenticated.
        command.Client.AuthenticationState = AuthenticationState.Authenticated;
        command.Client.AuthenticatedUser = user;
        command.Handled = true;

        // Let the client know their authentication was successful
        command.Client.SendSystemMessage(StringConstants.AUTHENTICATION_SUCCESS);

        // Let the server know this client has authenticated.
        gameService.ClientAuthenticated(command.Client);
    }
}
