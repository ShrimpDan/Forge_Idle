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

    [Header("Customer Spawn")]
    [SerializeField] private CustomerManager customerManager;

    public ForgeType ForgeType { get => forgeType; }
    public SceneType SceneType { get => sceneType; }
    public BlackSmith BlackSmith { get => blackSmith; }
    public CustomerManager CustomerManager { get => customerManager; }
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
        RecipeSystem = new WeaponRecipeSystem(this, gameManager.DataManager.CraftingLoader, gameManager.DataManager.RecipeLoader);
        StatHandler = new ForgeStatHandler(this, gameManager.DataManager);
        AssistantHandler = new ForgeAssistantHandler(this, VisualHandler, StatHandler);

        if (SellingSystem)
            SellingSystem.Init(this, gameManager.DataManager, gameManager.Inventory);

        if (blackSmith != null)
            blackSmith.Init();

        ForgeManager.ForgeTypeSaveSystem.LoadForge(this);
        CustomerManager.StartSpawnCustomer(this);
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
            EquippedAssistantKeys = AssistantHandler.GetSaveData(),
            Recipes = RecipeSystem.GetSaveData()
        };

        return data;
    }

    public void LoadFromData(ForgeTypeData data)
    {
        StatHandler.LoadFromData(data.UpgradeLevels);
        AssistantHandler.LoadFromData(data.EquippedAssistantKeys);
        RecipeSystem.LoadFormData(data.Recipes);
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
        ForgeManager.ForgeTypeSaveSystem.SaveForgeType(this);
        CustomerManager.StopSpawnCustomer();
        LoadSceneManager.Instance.UnLoadScene(SceneType);
    }

    public WeaponType GetRandomWeaponType()
    {
        if (ForgeWeaponTypeMapping.ForgeWeaponTypeDict.TryGetValue(ForgeType, out var weaponTypes))
        {
            if (weaponTypes.Length > 0)
                return weaponTypes[Random.Range(0, weaponTypes.Length)];
        }

        return default;
    }
}
