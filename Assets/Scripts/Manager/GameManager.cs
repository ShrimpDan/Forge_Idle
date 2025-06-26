using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Jang.InventoryManager Inventory { get; private set; }
    public DataManger TestDataManager { get; private set; }
    public TraineeManager AssistantManager { get; private set; }
    public Forge Forge { get; private set; }
    public UIManager UIManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Inventory = new Jang.InventoryManager();
        TestDataManager = new DataManger();

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
            var itemData = TestDataManager.ItemLoader.GetRandomItem();
            ItemInstance item = new ItemInstance();
            item.Data = itemData;
            Inventory.AddItem(item);
        }

        //Fabric 3개 "누적" 추가 (한 번에 추가)
        var fabricData = TestDataManager.ItemLoader.GetItemByKey("resource_fabric");
        if (fabricData != null)
        {
            ItemInstance fabricItem = new ItemInstance();
            fabricItem.Data = fabricData;
            Inventory.AddItem(fabricItem, 3);
            Debug.Log("<color=lime>[GameManager] Fabric 3개 추가 지급!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] resource_fabric 아이템 데이터가 없습니다!");
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

    [ContextMenu("Add Test Gold (5000)")]
    public void AddTestGold()
    {
        if (Forge != null)
        {
            Forge.AddGold(5000);
            Debug.Log("<color=yellow>[GameManager] 테스트용 골드 5000 지급!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] Forge 인스턴스가 없습니다!");
        }
    }
}
