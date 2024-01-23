using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDatabaseService databaseService;

    public AuthenticationService(IDatabaseService databaseService)
    {
        Log.Information("Initializing Authentication Service...");
        this.databaseService = databaseService;
    }

    public User? GetUser(string usernameOrEmail, string password)
    {
        return databaseService.GetUserForAuth(usernameOrEmail, password);
    }

    public void StartAuthentication(Client client)
    {
        client.AuthenticationState = AuthenticationState.WaitingForEmail;
        client.SendSystemMessage(StringConstants.AUTHENTICATION_PROMPT_USERNAME);
    }

}
