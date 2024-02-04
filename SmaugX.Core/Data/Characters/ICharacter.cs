using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Enums;

namespace SmaugX.Core.Data.Characters;

public interface ICharacter
{
    int Id { get; set; }
    int UserId { get; set; }
    string Name { get; set; }
    int Race { get; set; }
    int Class { get; set; }
    int CurrentRoomId { get; set; }
    Position Position { get; set; }

    Room? CurrentRoom { get; set; }

    public Task Tick();

}
