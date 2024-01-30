namespace SmaugX.Core.Constants;

/// <summary>
/// Holds constants related to character creation.
/// </summary>
internal class CharacterCreationConstants
{
    /// <summary>
    /// Played after the user has authenticated, but before they have a character.
    /// </summary>
    public const string CHARACTER_PROMPT_START = """
    You need a character to enter the game.
    Would you like to create a new character, or load an existing one?
    Type 'new' to create a new character, or 'load' to load an existing one.

    """;

    public const string CHARACTER_PROMPT_NEW_NAME = "Enter a name for your new character: ";
    public const string CHARACTER_PROMPT_NAME_TO_LOAD = "Enter the name of the character you want to load: ";
    public const string CHARACTER_PROMPT_NEW_OR_LOAD = "Type 'new' to create a new character, or 'load' to load an existing one.";
    public const string CHARACTER_NO_CHARACTERS_TO_LOAD = "You have no characters to load.";
    public const string CHARACTER_SELECT_HEADER = "Select a character to load:";
    public const string CHARACTER_NOT_FOUND = "Character not found.";
    public const string CHARACTER_ALREADY_EXISTS = "That character already exists.";

}
