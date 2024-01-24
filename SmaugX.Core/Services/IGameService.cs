using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public interface IGameService
{
    void UserJoined(Client client);
    void CharacterJoined(Character character);
    void ClientAuthenticated(Client client);
}
