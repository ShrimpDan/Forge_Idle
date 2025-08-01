using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoSingleton<GameManager>
{
    public InventoryManager Inventory { get; private set; }
    public DataManager DataManager { get; private set; }
    public AssistantManager AssistantManager { get; private set; }
    public AssistantInventory AssistantInventory => AssistantManager != null ? AssistantManager.AssistantInventory : null;
    public ForgeManager ForgeManager { get; private set; }
    public Forge Forge { get => ForgeManager.CurrentForge; }
    public SkillManager SkillManager { get; private set; }
    public WageProcessor WageProcessor { get; private set; }

    public UIManager UIManager { get; private set; }
    public DungeonSystem DungeonSystem { get; private set; }

    public List<AssistantInstance> HeldCandidates { get; private set; } = new();

    public CraftingManager CraftingManager { get; private set; }
    public GameSaveManager SaveManager { get; private set; }
    public TutorialManager TutorialManager { get; private set; }

    public CollectionBookManager CollectionManager { get; private set; }

    public DailyQuestManager DailyQuestManager { get; private set; }

    public WageCountdownUI WageCountdownUI { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        DataManager = new DataManager();
        Inventory = new InventoryManager(this);
        DungeonSystem = new DungeonSystem(this);
        WageProcessor = new WageProcessor(this);
        SkillManager = new SkillManager(DataManager.SkillDataLoader);
        WageCountdownUI = FindObjectOfType<WageCountdownUI>();

        ForgeManager = GetComponentInChildren<ForgeManager>();
        AssistantManager = FindObjectOfType<AssistantManager>();
        UIManager = FindObjectOfType<UIManager>();
        CollectionManager = FindAnyObjectByType<CollectionBookManager>();
        TutorialManager = FindObjectOfType<TutorialManager>();

        //일일 퀘스트
        DailyQuestManager= FindObjectOfType<DailyQuestManager>();
        if(DailyQuestManager != null)
            DailyQuestManager.Init(this);

        if (AssistantManager)
            AssistantManager.Init(this);

        if (ForgeManager)
            ForgeManager.Init(this);

        if (CollectionManager)
            CollectionManager.Init(this);

        if (UIManager)
            UIManager.Init(this);

        if (TutorialManager)
            TutorialManager.Init(this);

        // CraftingManager 동적 생성 및 초기화
        var cmObj = new GameObject("CraftingManager");
        CraftingManager = cmObj.AddComponent<CraftingManager>();
        CraftingManager.Init(Inventory, Forge);

        LoadSceneManager.Instance.SetMainCamera(Camera.main.gameObject);
        DontDestroyOnLoad(cmObj);
    }






    private void Start()
    {


        SaveManager = new GameSaveManager();

        SaveManager.RegisterSaveHandler(new SkillSaveHandler(SkillManager));
        SaveManager.RegisterSaveHandler(new InventorySaveHandler(Inventory));
        SaveManager.RegisterSaveHandler(new AssistantSaveHandler(AssistantManager, DataManager.PersonalityLoader));
        SaveManager.RegisterSaveHandler(new CollectionBookSaveHandler(CollectionManager)); //이거 수정해야될듯
        SaveManager.RegisterSaveHandler(new DungeonSaveHandler(DungeonSystem));
        SaveManager.RegisterSaveHandler(new HeldCandidateSaveHandler(this));
        SaveManager.RegisterSaveHandler(new ForgeSaveHandeler(ForgeManager));

        SaveManager.LoadAll();


        SoundManager.Instance.Play("MainBGM");
    }

    /// <summary>
    /// 강제로 시급 차감을 즉시 실행합니다.
    /// </summary>
    [ContextMenu("강제 시급 차감 실행")]
    public void DebugWageTick()
    {
        WageProcessor.ProcessHourlyWage();
        WageCountdownUI?.ResetTimer();
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
        if (ForgeManager != null)
        {
            ForgeManager.AddGold(5000);
            Debug.Log("<color=yellow>[GameManager] 테스트 골드 5000 지급 완료!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] Forge 인스턴스를 찾을 수 없습니다!");
        }
    }

    [ContextMenu("Add Test Gold (500000)")]
    public void AddTestGold2()
    {
        if (ForgeManager != null)
        {
            ForgeManager.AddGold(500000);
            Debug.Log("<color=yellow>[GameManager] 테스트 골드 500000 지급 완료!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] Forge 인스턴스를 찾을 수 없습니다!");
        }
    }

    [ContextMenu("Add Test Dia (500)")]
    public void AddTestDia500()
    {
        if (ForgeManager != null)
        {
            ForgeManager.AddDia(500);
            Debug.Log("<color=cyan>[GameManager] 테스트 다이아 500 지급 완료!</color>");
        }
        else
        {
            Debug.LogWarning("[GameManager] Forge 인스턴스를 찾을 수 없습니다!");
        }
    }

    [ContextMenu("Add Test Dia (750)")]
    public void AddTestDia750()
    {
        if (ForgeManager != null)
        {
            ForgeManager.AddDia(750);
            Debug.Log("<color=cyan>[GameManager] 테스트 다이아 750 지급 완료!</color>");
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

    [ContextMenu("Get Recipe Point")]
    public void GetRecipePoint()
    {
        ForgeManager.AddPoint(50);
    }

    [ContextMenu("Get Random Skill")]
    public void GetRandomSkill()
    {
        for(int i = 0; i < 10; i++)
            SkillManager.AddSkill(DataManager.SkillDataLoader.GetRandomSkill());
    }

    private void OnApplicationQuit()
    {
        DailyQuestManager?.SaveQuests();
        SaveManager.SaveAll();
    }
}
