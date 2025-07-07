using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public InventoryManager Inventory { get; private set; }
    public DataManager DataManager { get; private set; }
    public TraineeManager AssistantManager { get; private set; }
    public TraineeInventory TraineeInventory => AssistantManager != null ? AssistantManager.TraineeInventory : null;
    public Forge Forge { get; private set; }
    public UIManager UIManager { get; private set; }

    public DungeonData CurrentDungeon { get; private set; }
    public CraftingManager CraftingManager { get; private set; }

    public GameSaveManager SaveManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Inventory = new InventoryManager(this);
        DataManager = new DataManager();
        AssistantManager = FindObjectOfType<TraineeManager>();
        UIManager = FindObjectOfType<UIManager>();
        Forge = FindObjectOfType<Forge>();

        CollectionBookManager.Instance.InitDic();
        if (UIManager)
            UIManager.Init(this);
        if (Forge)
            Forge.Init(this);
        if (AssistantManager)
            AssistantManager.Init(this);

        // CraftingManager 동적 생성 및 초기화
        var cmObj = new GameObject("CraftingManager");
        CraftingManager = cmObj.AddComponent<CraftingManager>();
        CraftingManager.Init(Inventory, Forge);


        DontDestroyOnLoad(cmObj);
    }

    private void Start()
    {
        SaveManager = new GameSaveManager();

        SaveManager.RegisterSaveHandler(new ForgeSaveHandeler(Forge));
        SaveManager.RegisterSaveHandler(new InventorySaveHandler(Inventory));
        SaveManager.RegisterSaveHandler(new AssistantSaveHandler(AssistantManager, DataManager.PersonalityLoader));
        
        SaveManager.LoadAll();
    }


    [ContextMenu("Get Random Item")]
    public void GetRandomItem()
    {
        for (int i = 0; i < 20; i++)
        {
            var itemData = DataManager.ItemLoader.GetRandomItem();
            Inventory.AddItem(itemData);
        }
    }

    [ContextMenu("Get Random Assistant")]
    public void GetRandomAssi()
    {
        for (int i = 0; i < 20; i++)
        {
            AssistantManager.RecruitSingle();
        }
        Debug.Log("<color=yellow>[GameManager] 랜덤아이템 지급 완료!</color>");
    }

    [ContextMenu("Add Test Gold (5000)")]
    public void AddTestGold()
    {
        if (Forge != null)
        {
            Forge.AddGold(5000);
            Debug.Log("<color=yellow>[GameManager] 테스트 골드 5000 지급 완료!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] Forge 인스턴스를 찾을 수 없습니다!");
        }
    }

    [ContextMenu("Add All Resource Items (20 Each)")]
    public void AddAllResourcesTest()
    {
        int addedTypes = 0;
        foreach (var item in DataManager.ItemLoader.ItemList)
        {
            if (item.ItemKey != null && item.ItemKey.StartsWith("resource_"))
            {
                Inventory.AddItem(item, 20);
                addedTypes++;
            }
        }
        Debug.Log($"<color=cyan>[GameManager] 리소스 아이템 {addedTypes}종 20개씩 지급 완료!</color>");
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

    private void OnApplicationQuit()
    {
        SaveManager.SaveAll();
    }
}
