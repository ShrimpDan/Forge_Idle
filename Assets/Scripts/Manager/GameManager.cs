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

    

    protected override void Awake()
    {
        base.Awake();

        DataManager = new DataManager();
        Inventory = new InventoryManager(this);
        DungeonSystem = new DungeonSystem(this);
        WageProcessor = new WageProcessor(this);
        SkillManager = new SkillManager(DataManager.SkillDataLoader);

        ForgeManager = GetComponentInChildren<ForgeManager>();
        AssistantManager = FindObjectOfType<AssistantManager>();
        UIManager = FindObjectOfType<UIManager>();
        TutorialManager = FindObjectOfType<TutorialManager>();
        CollectionManager = FindAnyObjectByType<CollectionBookManager>();

        //일일 퀘스트
        DailyQuestManager= FindObjectOfType<DailyQuestManager>();
        if(DailyQuestManager != null)
        {
            DailyQuestManager.Init(this);
        }

        if (AssistantManager)
            AssistantManager.Init(this);

        if (ForgeManager)
            ForgeManager.Init(this);

        if (UIManager)
            UIManager.Init(this);

        if (TutorialManager)
            TutorialManager.Init(this);

        if (CollectionManager)
            CollectionManager.Init(this);



        // CraftingManager 동적 생성 및 초기화
        var cmObj = new GameObject("CraftingManager");
        CraftingManager = cmObj.AddComponent<CraftingManager>();
        CraftingManager.Init(Inventory, Forge);


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

        InvokeRepeating(nameof(ProcessWageWrapper), 5f, 5f);

        SoundManager.Instance.Play("MainBGM");
    }

    /// <summary>
    /// 해고되지 않은 모든 제자에게 시급을 지급.
    /// 지급 실패 시 IsFired = true 처리.
    /// 전체 지급 총합을 디버그 로그에 출력.
    /// </summary>
    private void ProcessWageWrapper()
    {
        WageProcessor.ProcessHourlyWage();
    }

    /// <summary>
    /// 강제로 시급 차감을 즉시 실행합니다.
    /// </summary>
    [ContextMenu("강제 시급 차감 실행")]
    public void DebugWageTick()
    {
        WageProcessor.ProcessHourlyWage();
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
        SaveManager.SaveAll();
    }
}
