using SmaugX.Core.Constants;

namespace SmaugX.Core.Data.World.Rooms;

internal class Room
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

    /*
        MAGENTA(*You moved North*)
        
        WHITE(*
        - Room Name
        
          This is the room description that is shown 
        when the player enters the room. It can be
        multiple lines long and will break at the
        correct screen width.

        CYAN(*Exits: North, South, East, West*)

    */
}
