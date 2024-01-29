using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

public interface IRoomService
{
    /// <summary>
    /// Called when a character joins a room.
    /// </summary>
    void CharacterJoined(Character character);

    Room GetRoomById(int id);
    int CreateRoom(Client client, string roomName);
    int CreateExit(Client client, string direction, int roomId);

    List<Exit> GetExitsByRoomId(int id);

    void MoveCharacter(Character character, Direction direction);
    void SendCharacterStatus(Character character);
    bool SetRoomName(Client client, string roomName);
    bool SetRoomDescription(Client client, string roomDescription);
    void Teleport(Client client, int roomId);
}
