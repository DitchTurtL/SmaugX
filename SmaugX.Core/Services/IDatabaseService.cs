using Npgsql;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

public interface IDatabaseService
{
    Character? GetCharacterByIdAndName(int id, string name);
    List<Character> GetCharactersByUserId(int id);
    Room? GetRoomById(int id);
    User? GetUserForAuth(string usernameOrEmail, string password);
}
