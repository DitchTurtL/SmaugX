namespace SmaugX.Core.Data.Items;

public interface IItem
{
    string Name { get; set; }
    string ShortDescription { get; set; }
    string LongDescription { get; set; }
    int Weight { get; set; }
    bool CanCarry { get; set; }
}
