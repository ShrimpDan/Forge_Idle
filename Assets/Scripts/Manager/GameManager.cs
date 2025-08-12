using System.Collections.Generic;
using UnityEngine;

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

    public WageCountdown WageCountdown { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        DataManager = new DataManager();
        Inventory = new InventoryManager(this);
        DungeonSystem = new DungeonSystem(this);
        WageProcessor = new WageProcessor(this);
        SkillManager = new SkillManager(DataManager.SkillDataLoader);
        WageCountdown = FindObjectOfType<WageCountdown>();

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

        HeldUIHelper.Instance?.UpdateCheckIcons();
    }

    /// <summary>
    /// 강제로 시급 차감을 즉시 실행합니다.
    /// </summary>
    public void DebugWageTick()
    {
        WageProcessor.ProcessHourlyWage();
        WageCountdown?.ResetTimer();
    }

    public void BuyDia(int amount)
    {
        if (ForgeManager != null)
        {
            ForgeManager.AddDia(amount);
        }
    }

    private void OnApplicationQuit()
    {
        DailyQuestManager?.SaveQuests();
        SaveManager.SaveAll();
    }
}
