using SmaugX.Core.Constants;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Hosting;

namespace SmaugX.Core.Services;

internal class CharacterCreationService
{
    internal static async Task StartCharacterCreation(Client client)
    {
        await client.SendSystemMessage(CharacterCreationConstants.CHARACTER_PROMPT_START);
    }
}
