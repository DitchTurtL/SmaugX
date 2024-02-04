using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.Items;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Enums;

namespace SmaugX.Core.Data.Characters;

public abstract class AbstractBasePlayerCharacter : IPlayerCharacter
{
    public Client? Client { get; set; }
    public virtual int Id { get; set; }
    public virtual int UserId { get; set; }
    public abstract string Name { get; set; }
    public virtual int Level { get; set; } = 1;
    public virtual Permissions Permissions { get; set; } = Permissions.Player;
    public virtual int Race { get; set; }
    public virtual int Class { get; set; }
    public virtual Position Position { get; set; }
    public virtual int CurrentRoomId { get; set; }
    public Room? CurrentRoom { get; set; }
    public virtual List<IItem> Inventory { get; set; } = new List<IItem>();


    public abstract bool HasPermission(Permissions builder);

    public abstract void SendStatus();

    public virtual Task Tick()
    {
        return Task.CompletedTask;
    }
}
