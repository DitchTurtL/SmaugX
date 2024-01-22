﻿using SmaugX.Core.Enums;
using SmaugX.Core.Helpers;

namespace SmaugX.Core.Constants;

internal class StringConstants
{
    // Authentication Prompts
    public const string AUTHENTICATION_PROMPT_USERNAME = "Enter your username or email: ";
    public const string AUTHENTICATION_PROMPT_PASSWORD = "Enter your password: ";
    public const string AUTHENTICATION_INVALID_CREDENTIALS = "Invalid credentials.";
    public const string AUTHENTICATION_SUCCESS = "Authentication successful.";

    // Default message colors
    public const string MESSAGE_COLOR_SYSTEM = "GREEN";
    public const string MESSAGE_COLOR_BANNER = "BLUE";
    public const string MESSAGE_COLOR_MOTD = "RED";
    public const string MESSAGE_COLOR_STATUS = "CYAN";

    // Random separator for messages
    public const string SEPARATOR = "--------------------------------------------------";

    public const string DEFAULT_ROOM_NAME = "an unknown location";
    public const string DEFAULT_CHARACTER_NAME = "Unknown";

    internal static string GetPosition(Position position)
    {
        return position switch
        {
            Position.Standing => "standing",
            Position.Floating => "floating",
            _ => ""
        };
    }

    internal static object GetSystemColor(MessageColor messageColor)
    {
        return messageColor switch
        {
            MessageColor.System => StringConstants.MESSAGE_COLOR_SYSTEM,
            MessageColor.Banner => StringConstants.MESSAGE_COLOR_BANNER,
            MessageColor.Motd => StringConstants.MESSAGE_COLOR_MOTD,
            MessageColor.Status => StringConstants.MESSAGE_COLOR_STATUS,
            _ => string.Empty
        };
    }
}
