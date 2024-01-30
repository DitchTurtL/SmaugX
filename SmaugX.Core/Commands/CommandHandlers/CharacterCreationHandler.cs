using SmaugX.Core.Constants;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.CommandHandlers;

internal class CharacterCreationHandler : ICommandHandler
{
    private readonly IDatabaseService databaseService;
    private readonly IRoomService roomService;

    public CharacterCreationHandler(IDatabaseService databaseService, IRoomService roomService)
    {
        this.databaseService = databaseService;
        this.roomService = roomService;
    }

    public void HandleCommand(ICommand command)
    {
        switch (command.Client.CharacterCreationState)
        {
            case CharacterCreationState.None:
                HandleNewAndLoad(command);
                break;
            case CharacterCreationState.WaitingForName:
                HandleName(command);
                break;
            case CharacterCreationState.Loading:
                HandleLoading(command);
                break;
        }
    }

    private void HandleNewAndLoad(ICommand command)
    {
        switch (command.Name.ToUpper())
        {
            case "NEW":
                command.Client.CharacterCreationState = CharacterCreationState.WaitingForName;
                command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NEW_NAME);
                command.Handled = true;
                break;
            case "LOAD":
                command.Client.CharacterCreationState = CharacterCreationState.Loading;

                // Get characters
                var characters = databaseService.GetCharactersByUserId(command.Client.AuthenticatedUser!.Id);
                var names = characters.Select(c => c.Name);

                // If no characters, start new character creation
                if (!names.Any())
                {
                    command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_NO_CHARACTERS_TO_LOAD);
                    command.Client.StartCharacterCreation(command.Client);
                    command.Handled = true;
                    return;
                }

                // Play character select header and list characters
                command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_SELECT_HEADER);
                command.Client.SendLines(names.ToArray());

                // Prompt for character name to load
                command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NAME_TO_LOAD);
                command.Handled = true;
                break;
            default:
                command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NEW_OR_LOAD);
                command.Handled = true;
                break;
        }
    }


    private void HandleName(ICommand command)
    {
        // if nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NEW_NAME);
            command.Handled = true;
            return;
        }

        // Check if the character already exists.
        var tmpCharacter = databaseService.GetCharacterByIdAndName(command.Client.AuthenticatedUser!.Id, command.Name);
        if (tmpCharacter != null)
        {
            command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_ALREADY_EXISTS);
            command.Client.StartCharacterCreation(command.Client);
            command.Handled = true;
            return;
        }

        // If the user supplied a name, create the character.
        var newCharacter = databaseService.CreateCharacter(command.Client.AuthenticatedUser.Id!, command.Name);
        HandleLoading(command);
    }

    /// <summary>
    /// Handles the loading of a character.
    /// </summary>
    private void HandleLoading(ICommand command)
    {
        // if nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NAME_TO_LOAD);
            command.Handled = true;
            return;
        }

        // Get selected character
        var character = databaseService.GetCharacterByIdAndName(command.Client.AuthenticatedUser!.Id, command.Name);
        // If no character found, prompt again.
        if (character == null)
        {
            command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_NOT_FOUND);
            command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NAME_TO_LOAD);
            command.Handled = true;
            return;
        }

        // Found character, load it
        command.Client.CharacterCreationState = CharacterCreationState.Loaded;
        character.roomService = roomService;
        if (character.CurrentRoomId <= 1)
        {
            character.CurrentRoomId = roomService.GetStartingRoomId();
        }



        command.Client.CharacterSelected(character);
        command.Handled = true;
    }
}
