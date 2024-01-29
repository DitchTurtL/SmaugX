using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

public interface IDatabaseService
{
    Character? GetCharacterByIdAndName(int id, string name);
    List<Character> GetCharactersByUserId(int id);
    Room? GetRoomById(int id);
    List<Exit> GetExitsByRoomId(int id);
    User? GetUserForAuth(string? usernameOrEmail, string password);
    int CreateRoom(string roomName, int userId);
    int CreateExit(int currentRoomId, Direction direction, int roomId, int userId);
    bool SetRoomName(int id, string roomName);
    bool SetRoomDescription(int id, string roomDescription);
    Task ExecuteScript(string contents);
}
