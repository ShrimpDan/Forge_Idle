using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Jang.InventoryManager Inventory { get; private set; }
    public DataManger DataManager { get; private set; }
    public TraineeManager AssistantManager { get; private set; }
    public Forge Forge { get; private set; }
    public UIManager UIManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Inventory = new Jang.InventoryManager();
        DataManager = new DataManger();

        AssistantManager = FindObjectOfType<TraineeManager>();
        UIManager = FindObjectOfType<UIManager>();

        Forge = FindObjectOfType<Forge>();

        if (UIManager)
            UIManager.Init(this);

        if (Forge)
            Forge.Init();
    }

    [ContextMenu("Get Random Item")]
    public void GetRandomItem()
    {
        for (int i = 0; i < 20; i++)
        {
            Debug.Log("Add Item!");
            var itemData = DataManager.ItemLoader.GetRandomItem();
            Inventory.AddItem(itemData);
        }
    }

    [ContextMenu("Get Random Assistant")]
    public void GetRandomAssi()
    {
        for (int i = 0; i < 20; i++)
        {
            AssistantManager.RecruitAndSpawnTrainee();
        }
    }
}
