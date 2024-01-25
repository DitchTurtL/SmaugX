using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Helpers;

namespace SmaugX.Core.Services;

public class DatabaseService : IDatabaseService
{
    private readonly SmaugXSettings configuration;

    public DatabaseService(IOptions<SmaugXSettings> configuration)
    {
        this.configuration = configuration.Value;
    }

    private string GetConnectionString() => configuration.ConnectionString;

    #region Characters

    public Character? GetCharacterByIdAndName(int id, string name)
    {
        return Task.Run<Character?>(async () => await GetCharacterByIdAndNameAsync(id, name)).Result;
    }

    private async Task<Character?> GetCharacterByIdAndNameAsync(int id, string name)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Get character with matching name and user id
        var query = "SELECT * FROM characters WHERE name = @name AND user_id = @id";
        var param = new { name, id };

        return await connection.QueryFirstOrDefaultAsync<Character>(query, param);
    }

    public List<Character> GetCharactersByUserId(int id)
    {
        return Task.Run<IEnumerable<Character>>(async () => await GetCharactersByUserIdAsync(id)).Result.ToList();
    }

    private async Task<IEnumerable<Character>> GetCharactersByUserIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Get characters with matching user id
        var query = "SELECT * FROM characters WHERE user_id = @id";
        var param = new { id };

        return await connection.QueryAsync<Character>(query, param);
    }

    #endregion

    #region Users

    public User? GetUserForAuth(string? usernameOrEmail, string password)
    {
        if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            return null;

        return Task.Run<User?>(async () => await GetUserForAuthAsync(usernameOrEmail, password)).Result;
    }

    private async Task<User?> GetUserForAuthAsync(string usernameOrEmail, string password)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
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

    #endregion

    #region Rooms

    public Room? GetRoomById(int id)
    {
        return Task.Run<Room?>(async () => await GetRoomByIdAsync(id)).Result;
    }

    private async Task<Room?> GetRoomByIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Get room with matching id
        var query = "SELECT * FROM rooms WHERE id = @id";
        var param = new { id };

        return await connection.QueryFirstOrDefaultAsync<Room>(query, param);
    }

    public List<Exit> GetExitsByRoomId(int id)
    {
        return Task.Run<IEnumerable<Exit>>(async () => await GetExitsByRoomIdAsync(id)).Result.ToList();
    }

    private async Task<List<Exit>> GetExitsByRoomIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Get exits with matching room id
        var query = "SELECT * FROM exits WHERE room_id = @id";
        var param = new { id };

        return (await connection.QueryAsync<Exit>(query, param)).ToList();
    }

    #endregion

}
