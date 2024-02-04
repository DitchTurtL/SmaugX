namespace SmaugX.Core.Data.Items.Weapons;

public class WoodenDagger : AbstractBaseItem
{
    public override string Name => "Wooden Dagger";
    public override string ShortDescription => "a finely honed dagger";
    public override string LongDescription => "You see a finely honed dagger here.";
    public override int Weight => 2;
    public override int Cost => 0;
    public override bool CanCarry => true;
}
