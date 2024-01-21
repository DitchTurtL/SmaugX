using Serilog;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Hosting;

namespace SmaugX.Core.Services;

internal static class GameService
{
    private static List<Client> AuthenticatedClients = new();
    private static List<Character> Characters = new();

    internal static async Task CharacterJoined(Client client)
    {
        var characterName = client.Character?.Name ?? "Unknown";

        Log.Information("Character Joined - {ipAddress}[{username}] as {characterName}",
            client.IpAddress, client.AuthenticatedUser!.Name, characterName);

        Characters.Add(client.Character!);

        await client.SendText($"Welcome back, {characterName}!");
    }

    /// <summary>
    /// Called once the client has authenticated and is ready to play the game.
    /// </summary>
    internal static async Task UserJoined(Client client)
    {
        Log.Information("User Joined - {ipAddress} as {username}[{email}]",
                       client.IpAddress, client.AuthenticatedUser?.Name ?? "Unknown",
                                  client.AuthenticatedUser?.Email ?? "Unknown");
        AuthenticatedClients.Add(client);

        await client.SendMotd();
    }
}
