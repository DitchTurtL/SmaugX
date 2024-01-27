using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmaugX.Core.Data.World.Rooms;

public class Exit
{
    public int Id { get; set; }

    public Direction Direction { get; set; }

    public int RoomId { get; set; }

    public int DestinationRoomId { get; set; }
}
