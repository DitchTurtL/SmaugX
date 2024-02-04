using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Serilog;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.Items;
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

    private string GetConnectionString()
    {
        var connStr = configuration.ConnectionString;
        if (string.IsNullOrEmpty(connStr))
            connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");

        if (string.IsNullOrEmpty(connStr))
            Log.Error("No connection string found.");

        return connStr;
    }

    #region Characters

    public Character? CreateCharacter(int userId, string characterName)
    {
        return Task.Run<Character?>(async () => await CreateCharacterAsync(userId, characterName)).Result;
    }

    private async Task<Character?> CreateCharacterAsync(int userId, string characterName)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Create character with matching name and user id
        var query = "INSERT INTO characters (name, user_id) VALUES (@characterName, @userId) RETURNING id";
        var param = new { characterName, userId };

        var id = await connection.QuerySingleAsync<int>(query, param);

        return new Character
        {
            Id = id,
            Name = characterName,
            UserId = userId
        };
    }


    public Character? GetCharacterByName(string name)
    {
        return Task.Run<Character?>(async () => await GetCharacterByNameAsync(name)).Result;
    }

    private async Task<Character?> GetCharacterByNameAsync(string name)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Get character with matching name
        var query = "SELECT * FROM characters WHERE name = @name";
        var param = new { name };

        return await connection.QueryFirstOrDefaultAsync<Character>(query, param);
    }

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

    public User? CreateUser(string username, string password)
    {
        return Task.Run<User?>(async () => await CreateUserAsync(username, password)).Result;
    }

    private async Task<User?> CreateUserAsync(string username, string password)
    {
        const string email = "none@none.com";

        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Create user with matching username
        var query = "INSERT INTO users (name, email, password) VALUES (@username, @email, @password) RETURNING id";
        var param = new { username, email, password = PasswordHasher.HashPassword(password) };

        var id = await connection.QuerySingleAsync<int>(query, param);

        return new User
        {
            Id = id,
            Name = username,
            Password = password
        };
    }


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
        var query = $"SELECT {Room.COLUMNS} FROM {Room.TABLE_NAME} WHERE id = @id";
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

    public int CreateRoom(string roomName, int userId)
    {
        return Task.Run<int>(async () => await CreateRoomAsync(roomName, userId)).Result;
    }

    private async Task<int> CreateRoomAsync(string roomName, int userId)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Create room with matching name
        var query = "INSERT INTO rooms (name, created_by) VALUES (@roomName, @userId) RETURNING id";
        var param = new { roomName, userId };

        return await connection.QuerySingleAsync<int>(query, param);
    }

    public int CreateExit(int currentRoomId, Direction direction, int roomId, int userId)
    {
        return Task.Run<int>(async () => await CreateExitAsync(currentRoomId, direction, roomId, userId)).Result;
    }

    private async Task<int> CreateExitAsync(int currentRoomId, Direction direction, int roomId, int userId)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Create exit with matching room id
        var query = "INSERT INTO exits (room_id, direction, destination_room_id, created_by) VALUES (@currentRoomId, @direction, @roomId, @userId) RETURNING id";
        var param = new { currentRoomId, direction, roomId, userId };

        return await connection.QuerySingleAsync<int>(query, param);
    }

    public bool SetRoomName(int id, string roomName)
    {
        return Task.Run<bool>(async () => await SetRoomNameAsync(id, roomName)).Result;
    }

    private async Task<bool> SetRoomNameAsync(int id, string roomName)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Update room with matching id
        var query = "UPDATE rooms SET name = @roomName WHERE id = @id";
        var param = new { id, roomName };

        return await connection.ExecuteAsync(query, param) > 0;
    }

    public bool SetRoomDescription(int id, string roomDescription)
    {
        return Task.Run<bool>(async () => await SetRoomDescriptionAsync(id, roomDescription)).Result;
    }

    private async Task<bool> SetRoomDescriptionAsync(int id, string roomDescription)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Update room with matching id
        var query = "UPDATE rooms SET short_description = @roomDescription WHERE id = @id";
        var param = new { id, roomDescription };

        return await connection.ExecuteAsync(query, param) > 0;
    }

    public async Task ExecuteScript(string contents)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        await connection.ExecuteAsync(contents);
    }




    #endregion

    public void CharacterMoved(int userId, int newRoomid)
    {
        Task.Run(async () => await CharacterMovedAsync(userId, newRoomid));
    }

    private async Task CharacterMovedAsync(int userId, int newRoomid)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Update character with matching user id
        var query = "UPDATE characters SET current_room_id = @newRoomid WHERE user_id = @userId";
        var param = new { userId, newRoomid };

        await connection.ExecuteAsync(query, param);
    }

    public bool UpdateUserPassword(int id, string newPassword)
    {
        return Task.Run<bool>(async () => await UpdateUserPasswordAsync(id, newPassword)).Result;
    }

    private async Task<bool> UpdateUserPasswordAsync(int id, string newPassword)
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Update user with matching id
        var query = "UPDATE users SET password = @newPassword WHERE id = @id";
        var param = new { id, newPassword = PasswordHasher.HashPassword(newPassword) };

        return await connection.ExecuteAsync(query, param) > 0;
    }

    public List<Item> GetWorldItems()
    {
        return Task.Run<IEnumerable<Item>>(async () => await GetWorldItemsAsync()).Result.ToList();
    }

    private async Task<IEnumerable<Item>> GetWorldItemsAsync()
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        connection.Open();

        // Get all items
        var query = "SELECT * FROM items";
        return await connection.QueryAsync<Item>(query);
    }
}
