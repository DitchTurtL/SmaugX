using System.Net;

namespace SmaugX.Core.Constants;

internal class SystemConstants
{
    // Hosting constants
    public static readonly IPAddress IP_ADDRESS = IPAddress.Any;
    public const int PORT = 4000;

    // Defaults
    public const int DEFAULT_STARTING_ROOM_ID = 2; // Something other than the void.

}
