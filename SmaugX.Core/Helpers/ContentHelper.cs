using SmaugX.Core.Constants;

namespace SmaugX.Core.Helpers;

internal static class ContentHelper
{
    private static string[]? banner = null;
    private static string[]? motd = null;

    /// <summary>
    /// Gets the Banner.dat file contents.
    /// This should be played to the client onConnect.
    /// </summary>
    internal static string[] Banner()
    {
        if (banner == null)
            banner = File.ReadAllLines(FileConstants.BANNER_FILE_PATH);

        return banner;
    }

    /// <summary>
    /// Gets the MOTD.dat file contents.
    /// This should be played to the client after authenticating.
    /// </summary>
    internal static string[] Motd()
    {
        if (motd == null)
            motd = File.ReadAllLines(FileConstants.MOTD_FILE_PATH);

        return motd;
    }
}
