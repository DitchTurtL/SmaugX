using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public interface IGameService
{
    Task UserJoined(Client client);
    Task CharacterJoined(Client client);
    void ClientConnected(Client client);
    void ClientExited(Client client);
    Task ClientAuthenticated(Client client);
}
