using SmaugX.Core.Data.Characters;

namespace SmaugX.Core.Data.Items.Weapons;

public class IronDagger : AbstractBaseItem
{
    public override string Name => "Iron Dagger";
    public override string ShortDescription => "a finely honed dagger";
    public override string LongDescription => "You see a finely honed dagger here.";
    public override int Weight => 2;
    public override int Cost => 0;
    public override bool CanCarry => true;
    public override EquipmentSlot Slot => EquipmentSlot.MainHand;
}
