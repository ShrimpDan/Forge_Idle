using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Inventory Inventory { get; private set; }
    public TestDataManger TestDataManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Inventory = new Inventory();
        TestDataManager = new TestDataManger();
    }

    [ContextMenu("Get Random Item")]
    public void GetRandomItem()
    {
        for (int i = 0; i < 20; i++)
        {
            Debug.Log("Add Item!");
            var itemData = TestDataManager.ItemLoader.GetRandomItem();

            ItemInstance item = new ItemInstance();
            item.Data = itemData;

            Inventory.AddItem(item);
        }
    }
}
