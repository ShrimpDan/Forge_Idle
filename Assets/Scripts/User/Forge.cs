using System.Collections.Generic;
using UnityEngine;

public class Forge : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] GameObject forgeMap;

    // 스탯
    private float craftSpeedMultiplier;
    private float rareItemChance;
    private float miningYieldPerMinute;
    private float maxMiningCapacity;
    private float sellPriceMultiplier;
    private float customerSpawnRate;


    // 보너스 스탯
    private float bonusCraftSpeedMultiplier = 1f;
    private float bonusRareItemChance = 0f;
    private float bonusMiningYieldPerMinute = 0f;
    private float bonusMaxMiningCapacity = 0f;
    private float bonusSellPriceMultiplier = 1f;
    private float bonusCustomerSpawnRate = 0f;

    // 최종 스탯
    public float FinalCraftSpeedMultiplier => craftSpeedMultiplier * bonusCraftSpeedMultiplier;
    public float FinalRareItemChance => rareItemChance + bonusRareItemChance;
    public float FinalMiningYieldPerMinute => miningYieldPerMinute + bonusMiningYieldPerMinute;
    public float FinalMaxMiningCapacity => maxMiningCapacity + bonusMaxMiningCapacity;
    public float FinalSellPriceMultiplier => sellPriceMultiplier * bonusSellPriceMultiplier;
    public float FinalCustomerSpawnRate => customerSpawnRate + bonusCustomerSpawnRate;


    // 레벨 & 명성치
    public int Level { get; private set; }
    public int CurrentFame { get; private set; }
    public int MaxFame { get; private set; }
    public int TotalFame { get; private set; }

    // 골드 & 다이아
    public int Gold { get; private set; }
    public int Dia { get; private set; }

    // 장착된 제자
    public Dictionary<SpecializationType, AssistantInstance> EquippedAssistant { get; private set; }

    [Header("Player")]
    [SerializeField] BlackSmith blackSmith;

    [Header("Assitant Roots")]
    [SerializeField] AssistantPrefabSO assistantPrefabSO;
    [SerializeField] Transform craftingSpawnRoot;
    [SerializeField] Transform miningSpawnRoot;
    [SerializeField] Transform sellingSpawnRoot;

    public BlackSmith BlackSmith { get => blackSmith; }
    public WeaponSellingSystem SellingSystem { get; private set; }
    public WeaponRecipeSystem RecipeSystem { get; private set; }

    // 이벤트 핸들러
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        SellingSystem = GetComponent<WeaponSellingSystem>();
        RecipeSystem = new WeaponRecipeSystem(this, gameManager.DataManager.CraftingLoader);

        if (SellingSystem)
            SellingSystem.Init(this, gameManager.DataManager);

        if (blackSmith != null)
            blackSmith.Init();

        InitAssistant();
    }

    private void InitAssistant()
    {
        EquippedAssistant = new Dictionary<SpecializationType, AssistantInstance>()
        {
            { SpecializationType.Crafting, null },
            { SpecializationType.Mining, null },
            { SpecializationType.Selling, null },
        };
    }

    private void RaiseAllEvents()
    {
        Events.RaiseGoldChanged(Gold);
        Events.RaiseDiaChanged(Dia);
        Events.RaiseFameChanged(CurrentFame, MaxFame);
        Events.RaiseLevelChanged(Level);
        Events.RasieTotalFameChanged(TotalFame);
    }


    public ForgeData SaveData()
    {
        ForgeData data = new ForgeData
        {
            // 제작 관련 스탯
            CraftSpeedMultiplier = craftSpeedMultiplier,
            RareItemChance = rareItemChance,

            // 채광 관련 스탯
            MiningYieldPerMinute = miningYieldPerMinute,
            MaxMiningCapacity = maxMiningCapacity,

            // 판매 관련 스탯
            SellPriceMultiplier = sellPriceMultiplier,
            CustomerSpawnRate = customerSpawnRate,

            // 일반 정보
            Level = Level,
            MaxFame = MaxFame,
            CurrentFame = CurrentFame,
            TotalFame = TotalFame,
            Gold = Gold,
            Dia = Dia
        };

        return data;
    }


    public void LoadData(ForgeData data)
    {
        // 제작 관련 스탯
        craftSpeedMultiplier = data.CraftSpeedMultiplier;
        rareItemChance = data.RareItemChance;

        // 채광 관련 스탯
        miningYieldPerMinute = data.MiningYieldPerMinute;
        maxMiningCapacity = data.MaxMiningCapacity;

        // 판매 관련 스탯
        sellPriceMultiplier = data.SellPriceMultiplier;
        customerSpawnRate = data.CustomerSpawnRate;

        // 일반 정보
        Level = data.Level;
        CurrentFame = data.CurrentFame;
        MaxFame = data.MaxFame;
        TotalFame = data.TotalFame;
        Gold = data.Gold;
        Dia = data.Dia;

        RaiseAllEvents();
    }

    public void AddFame(int amount)
    {
        CurrentFame += amount;
        TotalFame += amount;

        while (CurrentFame >= MaxFame)
        {
            Level++;
            CurrentFame -= MaxFame;
            MaxFame = (int)(MaxFame * 1.25f);

            Events.RaiseLevelChanged(Level);
        }

        Events.RaiseFameChanged(CurrentFame, MaxFame);
        Events.RasieTotalFameChanged(TotalFame);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Events.RaiseGoldChanged(Gold);
    }

    public void AddDia(int amount)
    {
        Dia += amount;
        Events.RaiseDiaChanged(Dia);
    }

    public bool UseGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            Events.RaiseGoldChanged(Gold);
            return true;
        }

        return false;
    }

    public bool UseDia(int amount)
    { 
        if (Dia >= amount)
        {
            Dia -= amount;
            Events.RaiseDiaChanged(Dia);
            return true;
        }

        return false;
    }

    public void ActiveAssistant(AssistantInstance assi)
    {
        // 이전에 등록된 제자가 있다면 해제
        AssistantInstance preAssi = EquippedAssistant[assi.Specialization];
        if (preAssi != null)
        {
            DeActiveAssistant(preAssi);
        }

        EquippedAssistant[assi.Specialization] = assi;
        Events.RaiseAssistantChanged(assi, true);
        assi.IsEquipped = true;

        ApplyAssistantStat(assi);
        SpawnAssistantPrefab(assi);
    }

    public void DeActiveAssistant(AssistantInstance assi)
    {
        assi.IsEquipped = false;
        EquippedAssistant[assi.Specialization] = null;
        Events.RaiseAssistantChanged(assi, false);

        DeApplyAssistantStat(assi);
        ClearSpawnRoot(assi.Specialization);
    }

    private void ApplyAssistantStat(AssistantInstance assi)
    {
        foreach (var stat in assi.Multipliers)
        {
            switch (stat.AbilityName)
            {
                case AssistantStatNames.IncreaseCraftSpeed:
                    bonusCraftSpeedMultiplier += stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseAdvancedCraftChance:
                    bonusRareItemChance += stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseMiningYieldPerMinute:
                    bonusMiningYieldPerMinute += stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseMaxMiningCapacity:
                    bonusMaxMiningCapacity += stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseSellPrice:
                    bonusSellPriceMultiplier += stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseCustomerCount:
                    bonusCustomerSpawnRate += stat.Multiplier;
                    break;
            }
        }
    }

    private void DeApplyAssistantStat(AssistantInstance assi)
    {
        foreach (var stat in assi.Multipliers)
        {
            switch (stat.AbilityName)
            {
                case AssistantStatNames.IncreaseCraftSpeed:
                    bonusCraftSpeedMultiplier -= stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseAdvancedCraftChance:
                    bonusRareItemChance -= stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseMiningYieldPerMinute:
                    bonusMiningYieldPerMinute -= stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseMaxMiningCapacity:
                    bonusMaxMiningCapacity -= stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseSellPrice:
                    bonusSellPriceMultiplier -= stat.Multiplier;
                    break;

                case AssistantStatNames.IncreaseCustomerCount:
                    bonusCustomerSpawnRate -= stat.Multiplier;
                    break;
            }
        }
    }


    private void SpawnAssistantPrefab(AssistantInstance assi)
    {
        Transform spawnRoot = null;

        switch (assi.Specialization)
        {
            case SpecializationType.Crafting:
                spawnRoot = craftingSpawnRoot;
                break;

            case SpecializationType.Mining:
                spawnRoot = miningSpawnRoot;
                break;

            case SpecializationType.Selling:
                spawnRoot = sellingSpawnRoot;
                break;
        }

        ClearSpawnRoot(assi.Specialization);
        Instantiate(assistantPrefabSO.GetAssistantByKey(assi.Key), spawnRoot);
    }

    private void ClearSpawnRoot(SpecializationType type)
    {
        Transform spawnRoot = null;

        switch (type)
        {
            case SpecializationType.Crafting:
                spawnRoot = craftingSpawnRoot;
                break;

            case SpecializationType.Mining:
                spawnRoot = miningSpawnRoot;
                break;

            case SpecializationType.Selling:
                spawnRoot = sellingSpawnRoot;
                break;
        }

        if (spawnRoot != null && spawnRoot.childCount > 0)
        {
            foreach (Transform child in spawnRoot)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void OpenForgeTab()
    {
        forgeMap.SetActive(true);
    }

    public void CloseForgeTab()
    {
        forgeMap.SetActive(false);
    }

    public void ClearForge()
    {
        foreach (Transform child in craftingSpawnRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in miningSpawnRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in sellingSpawnRoot)
        {
            Destroy(child.gameObject);
        }

        bonusCraftSpeedMultiplier = 1f;
        bonusRareItemChance = 0f;
        bonusMiningYieldPerMinute = 0f;
        bonusMaxMiningCapacity = 0f;
        bonusSellPriceMultiplier = 1f;
        bonusCustomerSpawnRate = 0f;
    }
}
