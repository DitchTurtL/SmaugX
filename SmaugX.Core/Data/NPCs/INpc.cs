namespace SmaugX.Core.Data.NPCs;

public interface INpc
{
    string Name { get; set; }
    string ShortDescription { get; set; }
    string LongDescription { get; set; }
    int Level { get; set; }
    int HitPoints { get; set; }
    int ManaPoints { get; set; }
    int MovementPoints { get; set; }


}
