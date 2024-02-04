namespace SmaugX.Core.Data.Items;

public abstract class AbstractBaseItem : IItem
{
    public int Id { get;  }
    public abstract string Name { get; }
    public abstract string ShortDescription { get; }
    public abstract string LongDescription { get; }
    public abstract int Weight { get; }
    public abstract int Cost { get; }
    public abstract bool CanCarry { get; }
}
