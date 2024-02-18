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
            new IronDagger(),
        };
        Items.AddRange(localItems);
    }

    /// <summary>
    /// Return an instance of an item by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IItem? GetItemById(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return null;

        var itemInstance = Activator.CreateInstance(item.GetType()) as IItem;
        return itemInstance;
    }





}
