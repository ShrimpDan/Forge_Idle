using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public InventoryManager Inventory { get; private set; }
    public DataManager DataManager { get; private set; }
    public AssistantManager AssistantManager { get; private set; }
    public AssistantInventory AssistantInventory => AssistantManager != null ? AssistantManager.AssistantInventory : null;
    public Forge Forge { get; private set; }
    public UIManager UIManager { get; private set; }
    public List<AssistantInstance> HeldCandidates { get; internal set; } = new();

    public DungeonSystem DungeonSystem{ get; private set; }

    public CraftingManager CraftingManager { get; private set; }

    public GameSaveManager SaveManager { get; private set; }

    public PoolManager PoolManager { get; private set; }

    public TutorialManager TutorialManager { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
       
        Inventory = new InventoryManager(this);
        DataManager = new DataManager();
        AssistantManager = FindObjectOfType<AssistantManager>();
        UIManager = FindObjectOfType<UIManager>();
        Forge = FindObjectOfType<Forge>();
        TutorialManager = FindObjectOfType<TutorialManager>();
        DungeonSystem = new DungeonSystem(this);
        PoolManager = PoolManager.Instance;

        CollectionBookManager.Instance.Initialize();
        if (UIManager)
            UIManager.Init(this);
        if (Forge)
            Forge.Init(this);
        if (AssistantManager)
            AssistantManager.Init(this);
        if (TutorialManager)
            TutorialManager.Init(this);
      
        // CraftingManager 동적 생성 및 초기화
        var cmObj = new GameObject("CraftingManager");
        CraftingManager = cmObj.AddComponent<CraftingManager>();
        CraftingManager.Init(Inventory, Forge);


        DontDestroyOnLoad(cmObj);
    }

    private void Start()
    {
        SaveManager = new GameSaveManager();

        // 세이브 핸들러 등록
        SaveManager.RegisterSaveHandler(new ForgeSaveHandeler(Forge));
        SaveManager.RegisterSaveHandler(new InventorySaveHandler(Inventory));
        SaveManager.RegisterSaveHandler(new AssistantSaveHandler(AssistantManager, DataManager.PersonalityLoader));
        SaveManager.RegisterSaveHandler(new WeaponSellingSaveHandler(Forge.SellingSystem));
        SaveManager.RegisterSaveHandler(new CollectionBookSaveHandler(CollectionBookManager.Instance));
        SaveManager.RegisterSaveHandler(new DungeonSaveHandler(DungeonSystem));
        SaveManager.RegisterSaveHandler(new HeldCandidateSaveHandler(this));


        SaveManager.LoadAll();

        InvokeRepeating(nameof(ProcessHourlyWage), 600f, 600f);
    }

    /// <summary>
    /// 장착 중이며 해고되지 않은 제자에게 시급을 지급.
    /// 지급 실패 시 해고 처리.
    /// 전체 지급 총합을 출력.
    /// </summary>
    public void ProcessHourlyWage()
    {
        if (AssistantInventory == null)
        {
            Debug.LogWarning("[시급] AssistantInventory가 null입니다.");
            return;
        }

        var activeTrainees = AssistantInventory.GetActiveTrainees();
        if (activeTrainees.Count == 0)
        {
            Debug.Log("<color=gray>[시급] 지급 대상이 없습니다.</color>");
            return;
        }

        int totalPaid = 0;

        foreach (var assi in activeTrainees)
        {
            if (Forge.UseGold(assi.Wage))
            {
                totalPaid += assi.Wage;
                Debug.Log($"[시급] {assi.Name}에게 {assi.Wage}G 지급 완료");
            }
            else
            {
                assi.IsFired = true;
                Debug.LogWarning($"[시급] {assi.Name}에게 {assi.Wage}G 지급 실패 → 해고 처리됨");
            }
        }

        if (totalPaid > 0)
        {
            Debug.Log($"<color=yellow>[시급 처리 완료] 총 {totalPaid}G 지출</color>");
        }

        SaveManager.SaveAll();
    }


    [ContextMenu("강제 시급 차감 실행")]
    public void DebugWageTick()
    {
        ProcessHourlyWage();
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

    [ContextMenu("Delete All SaveData")]
    public void DeleteSaveData()
    {
        SaveManager.DeleteAll();
        SaveManager.LoadAll();
    }

    private void OnApplicationQuit()
    {
       
            SaveManager.SaveAll();
       
    }
}
