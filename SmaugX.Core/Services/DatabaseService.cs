using Dapper;
using Npgsql;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Helpers;

namespace SmaugX.Core.Services;

internal static class DatabaseService
{
    internal static async Task<Character?> GetCharacterByNameWithUserId(string name, int id)
    {
        using var connection = new NpgsqlConnection(SystemConstants.CONNECTION_STRING);
        connection.Open();

        // Get character with matching name and user id
        var query = "SELECT * FROM characters WHERE name = @name AND user_id = @id";
        var param = new { name, id };

        return await connection.QueryFirstOrDefaultAsync<Character>(query, param);
    }

    internal static async Task<IEnumerable<Character>> GetCharactersByUserId(int id)
    {
        using var connection = new NpgsqlConnection(SystemConstants.CONNECTION_STRING);
        connection.Open();

        // Get characters with matching user id
        var query = "SELECT * FROM characters WHERE user_id = @id";
        var param = new { id };

        return await connection.QueryAsync<Character>(query, param);
    }

    internal static Room? GetRoomById(int id)
    {
        using var connection = new NpgsqlConnection(SystemConstants.CONNECTION_STRING);
        connection.Open();

        // Get room with matching id
        var query = "SELECT * FROM rooms WHERE id = @id";
        var param = new { id };

        return connection.QueryFirstOrDefault<Room>(query, param);
    }

    internal static async Task<User?> GetUser(string usernameOrEmail, string password)
    {
        using var connection = new NpgsqlConnection(SystemConstants.CONNECTION_STRING);
        connection.Open();

        // Get users with matching username or email
        var query = "SELECT * FROM users WHERE name = @nOrE OR email = @nOrE";
        var param = new { nOrE = usernameOrEmail };

        var users = await connection.QueryAsync<User>(query, param);

        foreach (var user in users)
        {
            // Check if password matches
            if (PasswordHasher.VerifyPassword(password, user.Password))
                return user;
        }
        return null;
    }
}
