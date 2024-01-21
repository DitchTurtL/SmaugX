using System.Net;

namespace SmaugX.Core.Constants;

internal class SystemConstants
{
    // Hosting constants
    public static readonly IPAddress IP_ADDRESS = IPAddress.Any;
    public const int PORT = 4000;

    // Database constants
    public static readonly string CONNECTION_STRING = $"Data Source={FileConstants.DATABASE_PATH};Version=3;";
}
