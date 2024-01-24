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
    public void CharacterJoined(Client client)
    {
        if (client.Character == null)
            Log.Error("Character is null, what broke?");

        var characterName = client.Character.Name;

        Log.Information("Character Joined - {ipAddress}[{username}] as {characterName}",
            client.IpAddress, client.AuthenticatedUser!.Name, characterName);

        // Add character to list of characters in the game.
        Characters.Add(client.Character);

        // Notify the Room Service that the character has entered the game.
        roomService.CharacterJoined(client.Character);

        // Send welcome message
        client.SendLine($"Welcome back, {characterName}!");

        // Separate all of the intro stuff from World stuff.
        client.SendSeparator();

        // Play the character's current status to the client.
        client.Character.ShowStatus();


    }



    public void ClientAuthenticated(Client client)
    {
        Log.Information("Client Authenticated - {ipAddress} as {username}[{email}]",
            client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
            client.AuthenticatedUser?.Email ?? "Unknown");

        UserJoined(client);
    }
}
