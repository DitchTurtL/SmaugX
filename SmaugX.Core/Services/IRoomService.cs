using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

public interface IRoomService
{
    void CharacterJoined(Character character);

    Room GetRoomById(int id);
    List<Exit> GetExitsByRoomId(int id);

    void MoveCharacter(Character character, Direction direction);
    void SendCharacterStatus(Character character);
}
