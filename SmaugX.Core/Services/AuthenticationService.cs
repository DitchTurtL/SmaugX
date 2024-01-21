using SmaugX.Core.Commands.Authentication;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Helpers;
using SmaugX.Core.Hosting;

namespace SmaugX.Core.Services;

internal static class AuthenticationService
{
    public static async Task<User?> GetUser(string usernameOrEmail, string password)
    {
        return await DatabaseService.GetUser(usernameOrEmail, password);
    }

    internal static async Task StartAuthentication(Client client)
    {
        client.AuthenticationState = AuthenticationState.WaitingForEmail;
        await client.SendText(StringConstants.AUTHENTICATION_PROMPT_USERNAME);
    }

}
