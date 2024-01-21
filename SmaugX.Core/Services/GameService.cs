using Serilog;
using SmaugX.Core.Hosting;

namespace SmaugX.Core.Services;

internal static class GameService
{
    private static List<Client> AuthenticatedClients = new();

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
