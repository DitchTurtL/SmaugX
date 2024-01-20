using SmaugX.Core.Constants;

namespace SmaugX.Core.Services;

internal class ContentService
{
    /// <summary>
    /// Gets the Banner.dat file contents.
    /// This should be played to the client onConnect.
    /// </summary>
    internal static string[] Banner()
    {
        return File.ReadAllLines(FileConstants.BANNER_FILE);
    }

    /// <summary>
    /// Gets the MOTD.dat file contents.
    /// This should be played to the client after authenticating.
    /// </summary>
    internal static string[] Motd()
    {
        return File.ReadAllLines(FileConstants.MOTD_FILE);
    }
}
