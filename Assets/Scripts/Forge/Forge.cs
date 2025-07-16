using UnityEngine;

public class Forge : MonoBehaviour
{
    private GameManager gameManager;
    public ForgeManager ForgeManager { get; private set; }

    [Header("Forge Settings")]
    [SerializeField] private ForgeType forgeType;
    [SerializeField] private SceneType sceneType;
    [SerializeField] private GameObject forgeMap;
    [SerializeField] private BlackSmith blackSmith;

    public ForgeType ForgeType { get => forgeType; }
    public SceneType SceneType { get => sceneType; }
    public BlackSmith BlackSmith { get => blackSmith; }
    public ForgeStatHandler StatHandler { get; private set; }
    public ForgeAssistantHandler AssistantHandler { get; private set; }
    public ForgeVisualHandler VisualHandler { get; private set; }
    public WeaponSellingSystem SellingSystem { get; private set; }
    public WeaponRecipeSystem RecipeSystem { get; private set; }

    // 이벤트 핸들러
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();

    private void Awake()
    {
        gameManager = GameManager.Instance;
        ForgeManager = gameManager.ForgeManager;
        
        ForgeManager.SetCurrentForge(this);

        VisualHandler = GetComponent<ForgeVisualHandler>();
        SellingSystem = GetComponent<WeaponSellingSystem>();
        RecipeSystem = new WeaponRecipeSystem(this, gameManager.DataManager.CraftingLoader);
        StatHandler = new ForgeStatHandler(this, gameManager.DataManager);
        AssistantHandler = new ForgeAssistantHandler(this, VisualHandler, StatHandler);

        if (SellingSystem)
            SellingSystem.Init(this, gameManager.DataManager);

        if (blackSmith != null)
            blackSmith.Init();

        CustomerManager.Instance.StartSpawnCustomer();
    }

    private void RaiseAllEvents()
    {
        Events.RaiseGoldChanged(ForgeManager.Gold);
        Events.RaiseDiaChanged(ForgeManager.Dia);
        Events.RaiseFameChanged(ForgeManager.CurrentFame, ForgeManager.MaxFame);
        Events.RaiseLevelChanged(ForgeManager.Level);
        Events.RasieTotalFameChanged(ForgeManager.TotalFame);
    }


    public ForgeTypeData SaveToData()
    {
        var data = new ForgeTypeData()
        {
            Type = ForgeType,
            UpgradeLevels = StatHandler.GetSaveData(),
            EquippedAssistantKeys = AssistantHandler.GetSaveData()
        };

        return data;
    }

    public void LoadFromData(ForgeTypeData data)
    {
        StatHandler.LoadFromData(data.UpgradeLevels);
        AssistantHandler.LoadFromData(data.EquippedAssistantKeys);
        RaiseAllEvents();
    }

    public void SetForgeMap(bool isAcitve)
    {
        forgeMap.SetActive(isAcitve);
    }

    public void CloseForgeTab()
    {
        forgeMap.SetActive(false);
    }

    public void ClearForge()
    {
        VisualHandler.ClearAllSpawnRoot();

        // 제자 스탯도 초기화
    }

    public void ExitForge()
    {
        CustomerManager.Instance.StopSpawnCustomer();
        LoadSceneManager.Instance.UnLoadScene(SceneType);
    }
}
