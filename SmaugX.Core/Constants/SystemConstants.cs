using System.Net;

namespace SmaugX.Core.Constants;

internal class SystemConstants
{
    // Hosting constants
    public static readonly IPAddress IP_ADDRESS = IPAddress.Any;
    public const int PORT = 4000;

    // Database constants
    private const string DATABASE_HOST = "192.168.0.202";
    private const int DATABASE_PORT = 5432;
    private const string DATABASE_NAME = "smaugx";
    private const string DATABASE_USERNAME = "smaugx";
    private const string DATABASE_PASSWORD = "smaugx";
    public static readonly string CONNECTION_STRING = $"Host={DATABASE_HOST};Port={DATABASE_PORT};Database={DATABASE_NAME};Username={DATABASE_USERNAME};Password={DATABASE_PASSWORD}";

    // Defaults
    public const int DEFAULT_STARTING_ROOM_ID = 1;

}
