using Serilog;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public class GameService : IGameService
{
    private readonly IRoomService roomService;

    private static List<Client> AuthenticatedClients = new();
    private static List<Character> Characters = new();

    public GameService(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    private Task Tick()
    {
        roomService.Tick();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Called once the client has authenticated and is ready to 
    /// pick a character or create a new one.
    /// </summary>
    public void UserJoined(Client client)
    {
        Log.Information("User Joined - {ipAddress} as {username}[{email}]",
                       client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
                                  client.AuthenticatedUser?.Email ?? "Unknown");

        AuthenticatedClients.Add(client);

        client.SendMotd();

        client.StartCharacterCreation(client);
    }

    /// <summary>
    /// Called once the client has selected a character to play.
    /// </summary>
    public void CharacterJoined(Character character)
    {
        if (character == null)
            Log.Error("Character is null, what broke?");

        var characterName = character!.Name;

        Log.Information("Character Joined - {ipAddress}[{username}] as {characterName}",
            character.Client!.IpAddress, character.Client!.AuthenticatedUser!.Name, characterName);

        // Add character to list of characters in the game.
        Characters.Add(character);

        // Notify the Room Service that the character has entered the game.
        roomService.CharacterJoined(character);

        // Send welcome message
        character.Client!.SendLine($"Welcome back, {characterName}!");

        // Separate all of the intro stuff from World stuff.
        character.Client!.SendSeparator();

        // Play the character's current status to the client.
        character.SendStatus();

    }

    public void ClientAuthenticated(Client client)
    {
        Log.Information("Client Authenticated - {ipAddress} as {username}[{email}]",
            client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
            client.AuthenticatedUser?.Email ?? "Unknown");

        UserJoined(client);
    }
}
