using Serilog;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Services;

public class GameService : IGameService
{
    private readonly IRoomService roomService;

    private static List<Client> Clients = new();
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
    public async Task UserJoined(Client client)
    {
        Log.Information("User Joined - {ipAddress} as {username}[{email}]",
                       client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
                                  client.AuthenticatedUser?.Email ?? "Unknown");
        AuthenticatedClients.Add(client);

        await client.SendMotd();

        client.StartCharacterCreation(client);
    }

    /// <summary>
    /// Called once the client has selected a character to play.
    /// </summary>
    public async Task CharacterJoined(Client client)
    {
        var characterName = client.Character.Name;

        Log.Information("Character Joined - {ipAddress}[{username}] as {characterName}",
            client.IpAddress, client.AuthenticatedUser!.Name, characterName);

        // Add character to list of characters in the game.
        Characters.Add(client.Character);

        // Notify the Room Service that the character has entered the game.
        roomService.CharacterJoined(client.Character);

        // Send welcome message
        await client.SendLine($"Welcome back, {characterName}!");

        // Separate all of the intro stuff from World stuff.
        await client.SendSeparator();

        // Play the character's current status to the client.
        await client.Character.ShowStatus();


    }

    public void ClientConnected(Client client)
    {
        Log.Information("Client connected - {ipAddress}", client.IpAddress);
        Clients.Add(client);
    }


    public void ClientExited(Client client)
    {
        Log.Information("Client Exited - {ipAddress}", client.IpAddress);
        Clients.Remove(client);
    }

    public async Task ClientAuthenticated(Client client)
    {
        Log.Information("Client Authenticated - {ipAddress} as {username}[{email}]",
            client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
            client.AuthenticatedUser?.Email ?? "Unknown");

        Clients.Remove(client);
        await UserJoined(client);
    }


}
