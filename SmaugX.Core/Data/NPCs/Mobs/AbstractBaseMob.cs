using SmaugX.Core.Enums;

namespace SmaugX.Core.Data.NPCs.Mobs;

internal abstract class AbstractBaseMob : IMob
{
    public virtual int Id { get; set; }
    public abstract string Name { get; set; }
    public abstract int Level { get; set; }
    public abstract bool Aggressive { get; set; }
    public abstract Position Position { get; set; }
    public abstract string ShortDescription { get; set; }
    public abstract string LongDescription { get; set; }
    public abstract string Description { get; set; }
    public abstract int Gold { get; set; }
    public abstract int Experience { get; set; }
    public abstract int HitPoints { get; set; }
    public abstract int RespawnTime { get; set; }

    public virtual Task Tick()
    {

        return Task.CompletedTask;
    }

}
