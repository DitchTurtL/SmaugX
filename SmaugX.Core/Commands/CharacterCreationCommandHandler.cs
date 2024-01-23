using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Services;

namespace SmaugX.Core.Commands;

internal class CharacterCreationCommandHandler : ICommandHandler
{
    public void HandleCommand(ICommand command)
    {
        var commandTask = command.Client.CharacterCreationState switch
        {
            CharacterCreationState.None => HandleNewAndLoad(command),
            CharacterCreationState.WaitingForName => HandleName(command),
            CharacterCreationState.WaitingForRace => HandleRace(command),
            CharacterCreationState.WaitingForClass => HandleClass(command),
            CharacterCreationState.Loading => HandleLoading(command),
            _ => HandleUnknown(command),
        };
        commandTask.Wait();

    }

    private async Task HandleNewAndLoad(ICommand command)
    {
        switch (command.Name.ToUpper())
        {
            case "NEW":
                command.Client.CharacterCreationState = CharacterCreationState.WaitingForName;
                await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NEW_NAME);
                command.Handled = true;
                break;
            case "LOAD":
                command.Client.CharacterCreationState = CharacterCreationState.Loading;

                // Get characters
                var characters = await command.Client.GetCharacters();
                var names = characters.Select(c => c.Name);

                // If no characters, start new character creation
                if (!names.Any())
                {
                    await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_NO_CHARACTERS_TO_LOAD);
                    command.Client.StartCharacterCreation(command.Client);
                    command.Handled = true;
                    return;
                }

                // Play character select header and list characters
                await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_SELECT_HEADER);
                await command.Client.SendLines(names.ToArray());

                // Prompt for character name to load
                await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NAME_TO_LOAD);
                command.Handled = true;
                break;
            default:
                await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NEW_OR_LOAD);
                command.Handled = true;
                break;
        }
    }

    /// <summary>
    /// Handles the loading of a character.
    /// </summary>
    private async Task HandleLoading(ICommand command)
    {
        // if nothing was supplied, prompt again.
        if (string.IsNullOrEmpty(command.Name))
        {
            await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NAME_TO_LOAD);
            command.Handled = true;
            return;
        }

        // Get selected character
        var character = await command.Client.GetCharacterByName(command.Name);
        // If no character found, prompt again.
        if (character == null)
        {
            await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_NOT_FOUND);
            await command.Client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_NAME_TO_LOAD);
            command.Handled = true;
            return;
        }

        // Found character, load it
        command.Client.CharacterCreationState = CharacterCreationState.Loaded;
        await command.Client.CharacterSelected(character);
        command.Handled = true;
    }

    private async Task HandleUnknown(ICommand command)
    {
        throw new NotImplementedException();
    }

    private async Task HandleClass(ICommand command)
    {
        throw new NotImplementedException();
    }

    private async Task HandleRace(ICommand command)
    {
        throw new NotImplementedException();
    }

    private async Task HandleName(ICommand command)
    {
        throw new NotImplementedException();
    }


}
