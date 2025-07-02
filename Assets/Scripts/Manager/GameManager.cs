using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Jang.InventoryManager Inventory { get; private set; }
    public DataManager DataManager { get; private set; }
    public TraineeManager AssistantManager { get; private set; }
    public TraineeInventory TraineeInventory => AssistantManager != null ? AssistantManager.TraineeInventory : null;
    public Forge Forge { get; private set; }
    public UIManager UIManager { get; private set; }

    public DungeonData CurrentDungeon { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Inventory = new Jang.InventoryManager();
        DataManager = new DataManager();
        AssistantManager = FindObjectOfType<TraineeManager>();
        UIManager = FindObjectOfType<UIManager>();
        Forge = FindObjectOfType<Forge>();

        if (UIManager)
            UIManager.Init(this);
        if (Forge)
            Forge.Init();
        if (AssistantManager)
            AssistantManager.Init(this);
    }

    [ContextMenu("Get Random Item")]
    public void GetRandomItem()
    {
        for (int i = 0; i < 20; i++)
        {
            var itemData = DataManager.ItemLoader.GetRandomItem();
            Inventory.AddItem(itemData);
        }


        var fabricData = DataManager.ItemLoader.GetItemByKey("resource_fabric");
        if (fabricData != null)
        {
            Inventory.AddItem(fabricData, 3);
            Debug.Log("<color=lime>[GameManager] Fabric 3�� �߰� ����!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] resource_fabric ������ �����Ͱ� �����ϴ�!");
        }
    }

    [ContextMenu("Get Random Assistant")]
    public void GetRandomAssi()
    {
        for (int i = 0; i < 20; i++)
        {
            AssistantManager.RecruitSingle();
        }
    }

    [ContextMenu("Add Test Gold (5000)")]
    public void AddTestGold()
    {
        if (Forge != null)
        {
            Forge.AddGold(5000);
            Debug.Log("<color=yellow>[GameManager] �׽�Ʈ�� ��� 5000 ����!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] Forge �ν��Ͻ��� �����ϴ�!");
        }
    }

    public void StartDungeon(DungeonData data)
    {
        CurrentDungeon = data;
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.Dungeon, true);
    }

    public void ExitDungeon()
    {
        CurrentDungeon = null;
    }
}
