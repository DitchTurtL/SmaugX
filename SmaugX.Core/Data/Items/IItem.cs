using SmaugX.Core.Data.Characters;

namespace SmaugX.Core.Data.Items;

public interface IItem
{
    int Id { get; }
    string Name { get; }
    string ShortDescription { get; }
    string LongDescription { get; }
    int Weight { get; }
    int Cost { get; }
    bool CanCarry { get; }
    EquipmentSlot Slot { get; }
}
