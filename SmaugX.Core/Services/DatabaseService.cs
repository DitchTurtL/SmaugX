using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using Dapper;
using SmaugX.Core.Helpers;
using Npgsql;
using SmaugX.Core.Data.Characters;

namespace SmaugX.Core.Services;

internal static class DatabaseService
{
    internal static Task<Character?> GetCharacterByNameWithUserId(string name, int id)
    {
        using var connection = new NpgsqlConnection(SystemConstants.CONNECTION_STRING);
        connection.Open();

        // Get character with matching name and user id
        var query = "SELECT * FROM characters WHERE name = @name AND user_id = @id";
        var param = new { name, id };

        return connection.QueryFirstOrDefaultAsync<Character>(query, param);
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
