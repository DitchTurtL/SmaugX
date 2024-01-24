using SmaugX.Core.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmaugX.Core.Data.World.Rooms;

public class Room
{
    public int Id { get; set; }

    /// <summary>
    /// The reference ID of the room.
    /// This is a ULID that is used to reference this room in the database.
    /// This should help with long term world integrity also if we need to move the data around.
    /// </summary>
    public string ReferenceId { get; set; } = string.Empty;
    public string Name { get; set; } = StringConstants.DEFAULT_ROOM_NAME;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;

    [NotMapped]
    public List<Exit?> Exits { get; set; } = null!;

}
