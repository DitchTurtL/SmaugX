using SmaugX.Core.Enums;

namespace SmaugX.Core.Data.NPCs.Mobs;

internal interface IMob
{
    int Id { get; set; }
    string Name { get; set; }
    int Level { get; set; }
	bool Aggressive { get; set; }
	Position Position { get; set; }
	string ShortDescription { get; set; }
	string LongDescription { get; set; }
	string Description { get; set; }
	int Gold { get; set; }
	int Experience { get; set; }
	int HitPoints { get; set; }
	int RespawnTime { get; set; }

    Task Tick();

}