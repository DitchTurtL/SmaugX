using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public interface IGameService
{
    void UserJoined(Client client);
    void CharacterJoined(Client client);
    void ClientConnected(Client client);
    void ClientExited(Client client);
    void ClientAuthenticated(Client client);
}
