using SmaugX.Core.Enums;

namespace SmaugX.Core.Data.NPCs.Mobs;

internal class GreyRat : AbstractBaseMob
{
    public override string Name { get; set; } = "Grey Rat";
    public override int Level { get; set; } = 1;
    public override bool Aggressive { get; set; } = false;
    public override Position Position { get; set; } = Position.Crawling;
    public override string ShortDescription { get; set; } = "a large grey rat";
    public override string LongDescription { get; set; } = "A large rat scuttles across your path, eyes glowing in the dark.";
    public override string Description { get; set; } = "This is one large rat. You would not want to allow him to bite you.\nHis grey fur is matted from blood and dirt, and his eyes seem to\nwatch your every movement.";
    public override int Gold { get; set; } = 5;
    public override int Experience { get; set; } = 10;
    public override int HitPoints { get; set; } = 10;
    public override int RespawnTime { get; set; } = 60;

    public override Task Tick()
    {

        return base.Tick();
    }
}
