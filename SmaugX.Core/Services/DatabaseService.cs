using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using System.Data.SQLite;
using Dapper;
using SmaugX.Core.Helpers;

namespace SmaugX.Core.Services;

internal static class DatabaseService
{
    internal static async Task<User?> GetUser(string usernameOrEmail, string password)
    {
        using var connection = new SQLiteConnection(SystemConstants.CONNECTION_STRING);
        connection.Open();

        // Get users with matching username or email
        var query = "SELECT * FROM Users WHERE Name = @nOrE OR Email = @nOrE";
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
