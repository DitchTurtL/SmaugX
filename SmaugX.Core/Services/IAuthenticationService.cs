using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public interface IAuthenticationService
{
    User? GetUser(string usernameOrEmail, string password);

    void StartAuthentication(Client client);
}
