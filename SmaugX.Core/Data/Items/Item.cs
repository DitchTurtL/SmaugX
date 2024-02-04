namespace SmaugX.Core.Data.Items;

public class Item : IItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public int Weight { get; set; }
    public bool CanCarry { get; set; }
}
