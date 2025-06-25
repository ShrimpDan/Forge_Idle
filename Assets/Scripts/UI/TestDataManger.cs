public class TestDataManger
{
    public ItemDataLoader ItemLoader { get; private set; }
    public CraftingDataLoader CraftingLoader{ get; private set; }

    public TestDataManger()
    {
        ItemLoader = new ItemDataLoader();
        CraftingLoader = new CraftingDataLoader();
    }
}
