using SmaugX.Core.Constants;
using System.Text;

namespace SmaugX.Core.Helpers;

internal enum Color
{
    None = 0,
    Black = 30,
    Red = 31,
    Green = 32,
    Yellow = 33,
    Blue = 34,
    Magenta = 35,
    Cyan = 36,
    White = 37
}

internal enum BackgroundColor
{
    None = 0,
    Black = 40,
    Red = 41,
    Green = 42,
    Yellow = 43,
    Blue = 44,
    Magenta = 45,
    Cyan = 46,
    White = 47
}

internal static class Colors
{
    private const string ESC = "\u001b[";
    private const string ESC_END = "m";
    private static readonly string RESET = $"{ESC}{(int)Color.None}{ESC_END}";
    private static readonly string BLACK = $"{ESC}{(int)Color.Black}{ESC_END}";
    private static readonly string RED = $"{ESC}{(int)Color.Red}{ESC_END}";
    private static readonly string GREEN = $"{ESC}{(int)Color.Green}{ESC_END}";
    private static readonly string YELLOW = $"{ESC}{(int)Color.Yellow}{ESC_END}";
    private static readonly string BLUE = $"{ESC}{(int)Color.Blue}{ESC_END}";
    private static readonly string MAGENTA = $"{ESC}{(int)Color.Magenta}{ESC_END}";
    private static readonly string CYAN = $"{ESC}{(int)Color.Cyan}{ESC_END}";
    private static readonly string WHITE = $"{ESC}{(int)Color.White}{ESC_END}";

    private static readonly string BLACK_BG = $"{ESC}{(int)BackgroundColor.Black}{ESC_END}";
    private static readonly string RED_BG = $"{ESC}{(int)BackgroundColor.Red}{ESC_END}";
    private static readonly string GREEN_BG = $"{ESC}{(int)BackgroundColor.Green}{ESC_END}";
    private static readonly string YELLOW_BG = $"{ESC}{(int)BackgroundColor.Yellow}{ESC_END}";
    private static readonly string BLUE_BG = $"{ESC}{(int)BackgroundColor.Blue}{ESC_END}";
    private static readonly string MAGENTA_BG = $"{ESC}{(int)BackgroundColor.Magenta}{ESC_END}";
    private static readonly string CYAN_BG = $"{ESC}{(int)BackgroundColor.Cyan}{ESC_END}";
    private static readonly string WHITE_BG = $"{ESC}{(int)BackgroundColor.White}{ESC_END}";

    /// <summary>
    /// Colorizes strings based on text formatting.
    /// #BLACK(*text*) - Black text
    /// #RED:GREEN(*text*) - Red text, green background
    /// #0 - Reset all formatting
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Colorize(string text, MessageColor messageColor = MessageColor.None)
    {
        // if we're not colorizing manually, check for a message color.
        if (!text.Contains('#'))
        {
            if (messageColor == MessageColor.None)
                return text;

            // Override color based on message color.
            return ColorizeString(text, messageColor);
        }

        var sb = new StringBuilder();
        for (var i = 0; i< text.Length; i++)
        {
            var curChar = text[i];

            // If we're not colorizing, just append the character.
            if (curChar != '#') 
            {
                sb.Append(curChar);
                continue;
            }

            // If we're colorizing, check for a reset and append it if it exists.
            var nextChar = i + 1 < text.Length ? text[i + 1] : '\0';
            if (nextChar == '0')
            {
                sb.Append(RESET);
                i++; // Skip the reset character.
                continue;
            }

            // If it's not a reset, it's a color.
            // Get the portion of the string to colorize.
            // #RED:GREEN(*text*)
            var colorizeStart = i + 1;
            var colorizeEnd = text.IndexOf("*)", colorizeStart) + 2;
            var colorizeString = text.Substring(colorizeStart, colorizeEnd - colorizeStart);
            var colorizedText = ColorizeString(colorizeString);
            sb.Append(colorizedText);
            i = colorizeEnd; // Skip the colorized string and the closing characters.
        }

        return sb.ToString();
    }

    /// <summary>
    /// Replaces color tags with the appropriate ESC sequences.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string ColorizeString(string text, MessageColor messageColor = MessageColor.None)
    {
        // If a message color is specified, wrap the text in the appropriate color tags.
        if (messageColor != MessageColor.None)
        {
            var color = messageColor switch
            {
                MessageColor.System => StringConstants.MESSAGE_COLOR_SYSTEM,
                MessageColor.Banner => StringConstants.MESSAGE_COLOR_BANNER,
                MessageColor.Motd => StringConstants.MESSAGE_COLOR_MOTD,
                _ => string.Empty
            };

            text = $"{color}(*{text}*)";
        }

        var colorTag = text.Substring(0, text.IndexOf("(*"));
        var colorTagParts = colorTag.Split(':');

        var foreground = GetColor(colorTagParts[0]);
        var background = colorTagParts.Length > 1 ? GetBackgroundColor(colorTagParts[1]) : BackgroundColor.None;

        var colorTagEnd = colorTag.Length + 2;
        var textToColorize = text.Substring(colorTagEnd, text.Length - 2 - colorTagEnd);

        var fgText = $"{ESC}{(int)foreground}{ESC_END}";
        var bgText = (int)background > 0 ? $"{ESC}{(int)background}{ESC_END}" : string.Empty;

        var sb = new StringBuilder();
        sb.Append(fgText);
        sb.Append(bgText);
        sb.Append(textToColorize);
        var reset = RESET;
        sb.Append(RESET);
        return sb.ToString();
    }

    private static string GetForeground(Color color)
    {
        return color switch
        {
            Color.Black => BLACK,
            Color.Red => RED,
            Color.Green => GREEN,
            Color.Yellow => YELLOW,
            Color.Blue => BLUE,
            Color.Magenta => MAGENTA,
            Color.Cyan => CYAN,
            Color.White => WHITE,
            _ => string.Empty
        };
    }

    private static string GetBackground(Color color)
    {
        return color switch
        {
            Color.Black => BLACK_BG,
            Color.Red => RED_BG,
            Color.Green => GREEN_BG,
            Color.Yellow => YELLOW_BG,
            Color.Blue => BLUE_BG,
            Color.Magenta => MAGENTA_BG,
            Color.Cyan => CYAN_BG,
            Color.White => WHITE_BG,
            _ => string.Empty
        };
    }

    /// <summary>
    /// Returns the foreground color associated with the string.
    /// </summary>
    private static Color GetColor(string color)
    {
        return color switch
        {
            "BLACK" => Color.Black,
            "RED" => Color.Red,
            "GREEN" => Color.Green,
            "YELLOW" => Color.Yellow,
            "BLUE" => Color.Blue,
            "MAGENTA" => Color.Magenta,
            "CYAN" => Color.Cyan,
            "WHITE" => Color.White,
            _ => Color.None
        };
    }

    /// <summary>
    /// Returns the background color assiciated with the string.
    /// </summary>\
    private static BackgroundColor GetBackgroundColor(string color)
    {
        return color switch
        {
            "BLACK" => BackgroundColor.Black,
            "RED" => BackgroundColor.Red,
            "GREEN" => BackgroundColor.Green,
            "YELLOW" => BackgroundColor.Yellow,
            "BLUE" => BackgroundColor.Blue,
            "MAGENTA" => BackgroundColor.Magenta,
            "CYAN" => BackgroundColor.Cyan,
            "WHITE" => BackgroundColor.White,
            _ => BackgroundColor.None
        };
    }
    /*
    Foreground Colors:
        Black: \u001b[30m
        Red: \u001b[31m
        Green: \u001b[32m
        Yellow: \u001b[33m
        Blue: \u001b[34m
        Magenta: \u001b[35m
        Cyan: \u001b[36m
        White: \u001b[37m
    Background Colors:
        Black: \u001b[40m
        Red: \u001b[41m
        Green: \u001b[42m
        Yellow: \u001b[43m
        Blue: \u001b[44m
        Magenta: \u001b[45m
        Cyan: \u001b[46m
        White: \u001b[47m
    Reset (To Default):
        Reset all formatting: \u001b[0m
    */



}
