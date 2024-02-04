using SmaugX.Core.Constants;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Items;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmaugX.Core.Data.World.Rooms;

public class Room
{
    public const string TABLE_NAME = "rooms";
    public const string COLUMNS = "id, name, short_description, long_description, created_by";

    public int Id { get; set; }

    /// <summary>
    /// The reference ID of the room.
    /// This is a ULID that is used to reference this room in the database.
    /// This should help with long term world integrity also if we need to move the data around.
    /// </summary>
    public string Name { get; set; } = StringConstants.DEFAULT_ROOM_NAME;

    public string ShortDescription { get; set; } = string.Empty;

    public string LongDescription { get; set; } = string.Empty;

    public int CreatedBy { get; set; } = 0;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public List<Exit?> Exits { get; set; } = null!;
    public List<Character> Characters { get; set; } = new();
    public List<IItem> Inventory { get; set; } = new();

    public Task Tick()
    {
        foreach (var character in Characters)
            character.Tick();


        return Task.CompletedTask;
    }

    public void CharacterLeft(Character character)
    {
        Characters.Remove(character);

        // Notify the room that the character has left.
        foreach (var c in Characters)
            c.Client!.SendSystemMessage($"{character.Name} left the room.");
    }

    public void CharacterEntered(Character character)
    {
        character.CurrentRoom = this;
        character.CurrentRoomId = Id;

        // Notify the room that the character has entered.
        foreach (var c in Characters)
        {
            c.Client!.SendSystemMessage($"{character.Name} entered the room.");
        }

        Characters.Add(character);

        character.SendStatus();
    }

    internal void Say(Character character, string message)
    {
        foreach (var c in Characters)
        {
            if (c != character)
                c.Client!.SendSystemMessage($"{character.Name} says, \"{message}\"");
            else
                c.Client!.SendSystemMessage($"You say, \"{message}\"");
        }
    }
}
