namespace SmaugX.Core.Constants;

public static class FileConstants
{
    // File logging
    public const string LOG_DIRECTORY = "Logs";
    public const string LOG_FILE = "log.txt";

    // Content
    public const string CONTENT_DIRECTORY = "Content";
    public static readonly string CONTENT_PATH = Path.Combine(Environment.CurrentDirectory, CONTENT_DIRECTORY);
    public static readonly string HOSTING_CONTENT_PATH = Path.Combine(CONTENT_PATH, "Hosting");
    public const string BANNER_FILE = "Banner.dat";
    public static readonly string BANNER_FILE_PATH = Path.Combine(HOSTING_CONTENT_PATH, BANNER_FILE);
    public const string MOTD_FILE = "MOTD.dat";
    public static readonly string MOTD_FILE_PATH = Path.Combine(HOSTING_CONTENT_PATH, MOTD_FILE);

    // Database
    public const string SQL_DIRECTORY = "SQLScripts";
    public static readonly string SQL_PATH = Path.Combine(Environment.CurrentDirectory, SQL_DIRECTORY);

}
