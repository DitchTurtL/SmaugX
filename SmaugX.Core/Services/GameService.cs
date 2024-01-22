using Serilog;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Hosting;

namespace SmaugX.Core.Services;

internal static class GameService
{
    private static List<Client> AuthenticatedClients = new();
    private static List<Character> Characters = new();

    /// <summary>
    /// Called once the client has authenticated and is ready to 
    /// pick a character or create a new one.
    /// </summary>
    internal static async Task UserJoined(Client client)
    {
        Log.Information("User Joined - {ipAddress} as {username}[{email}]",
                       client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
                                  client.AuthenticatedUser?.Email ?? "Unknown");
        AuthenticatedClients.Add(client);

        await client.SendMotd();

        await CharacterCreationService.StartCharacterCreation(client);
    }

    /// <summary>
    /// Called once the client has selected a character to play.
    /// </summary>
    internal static async Task CharacterJoined(Client client)
    {
        var characterName = client.Character.Name;

        Log.Information("Character Joined - {ipAddress}[{username}] as {characterName}",
            client.IpAddress, client.AuthenticatedUser!.Name, characterName);

        // Add character to list of characters in the game.
        Characters.Add(client.Character);

        // Notify the Room Service that the character has entered the game.
        await RoomService.CharacterJoined(client.Character);

        // Send welcome message
        await client.SendLine($"Welcome back, {characterName}!");

        // Separate all of the intro stuff from World stuff.
        await client.SendSeparator();

        // Play the character's current status to the client.
        await client.Character.ShowStatus();


    }


}
