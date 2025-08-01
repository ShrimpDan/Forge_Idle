using System.Collections.Generic;
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

    private void Awake()
    {
        gameManager = GameManager.Instance;
        ForgeManager = gameManager.ForgeManager;

        ForgeManager.SetCurrentForge(this);

        VisualHandler = GetComponent<ForgeVisualHandler>();
        SellingSystem = GetComponent<WeaponSellingSystem>();
        RecipeSystem = new WeaponRecipeSystem(this, gameManager.DataManager.CraftingLoader, gameManager.DataManager.RecipeLoader);
        StatHandler = new ForgeStatHandler(this, gameManager.DataManager);
        AssistantHandler = new ForgeAssistantHandler(this);

        if (SellingSystem)
            SellingSystem.Init(this, gameManager.Inventory);

        if (blackSmith != null)
            blackSmith.Init();

        if (!ForgeManager.EquippedAssistant.ContainsKey(ForgeType))
        {
            ForgeManager.EquippedAssistant[ForgeType] = new Dictionary<SpecializationType, AssistantInstance>();
        }

        ForgeManager.ForgeTypeSaveSystem.LoadForge(this);
        CustomerManager.StartSpawnCustomer(this);
    }

    public ForgeTypeData SaveToData()
    {
        var data = new ForgeTypeData()
        {
            Type = ForgeType,
            UpgradeLevels = StatHandler.GetSaveData(),
            Recipes = RecipeSystem.GetSaveData()
        };

        return data;
    }

    public void LoadFromData(ForgeTypeData data)
    {
        StatHandler.LoadFromData(data.UpgradeLevels);
        RecipeSystem.LoadFormData(data.Recipes);
    }

    public void SetForgeMap(bool isAcitve)
    {
        forgeMap.SetActive(isAcitve);
    }

    public void ExitForge()
    {
        ForgeManager.SkillSystem.StopAllSkillEffectCoroutine();
        CustomerManager.StopSpawnCustomer();

        ForgeManager.ForgeTypeSaveSystem.SaveForgeType(this, () =>
        {
            LoadSceneManager.Instance.UnLoadScene(SceneType);
        });
    }

    
}
