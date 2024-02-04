using SmaugX.Core.Data.Items;
using SmaugX.Core.Data.Items.Weapons;

namespace SmaugX.Core.Services;

public class ItemService : IItemService
{
    private readonly IDatabaseService databaseService;

    private List<IItem> Items { get; set; } = new List<IItem>();

    public ItemService(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;

        LoadWorldItems();
    }

    private async Task LoadWorldItems()
    {
        // Load items from the database
        var items = databaseService.GetWorldItems();

        foreach (var item in items)
            Items.Add(item);

        // Load coded items
        var localItems = new List<IItem>
        {
            new WoodenDagger(),
        };
        Items.AddRange(localItems);
        


    }

    public IItem GetItemByName(string name)
    {
        return Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public IItem GetItemById(int id)
    {
        return Items.FirstOrDefault(x => x.Id == id);
    }





}
