public class DataManger
{
    public ItemDataLoader ItemLoader { get; private set; }
    public CraftingDataLoader CraftingLoader{ get; private set; }
    public DungeonDataLoader DungeonDataLoader { get; private set; }

    public DataManger()
    {
        ItemLoader = new ItemDataLoader();
        CraftingLoader = new CraftingDataLoader();
        DungeonDataLoader = new DungeonDataLoader();
    }
}
