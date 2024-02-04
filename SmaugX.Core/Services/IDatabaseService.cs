using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Items;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

public interface IDatabaseService
{
    Character? GetCharacterByName(string name);
    Character? GetCharacterByIdAndName(int id, string name);
    Character? CreateCharacter(int userId, string characterName);
    List<Character> GetCharactersByUserId(int id);
    void CharacterMoved(int userId, int newRoomid);
    Room? GetRoomById(int id);
    List<Exit> GetExitsByRoomId(int id);
    User? GetUserForAuth(string? usernameOrEmail, string password);
    bool UpdateUserPassword(int id, string newPassword);
    int CreateRoom(string roomName, int userId);
    int CreateExit(int currentRoomId, Direction direction, int roomId, int userId);
    bool SetRoomName(int id, string roomName);
    bool SetRoomDescription(int id, string roomDescription);
    Task ExecuteScript(string contents);
    User? CreateUser(string username, string password);
    List<Item> GetWorldItems();
}
