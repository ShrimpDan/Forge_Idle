using System.Collections.Generic;
using UnityEngine;

public class Forge : MonoBehaviour
{
    private GameManager gameManager;
    private ForgeData forgeData;

    // 스탯
    private float craftSpeedMultiplier;
    private float rareItemChance;
    private float enhanceSuccessRate;
    private float breakChanceReduction;
    private float enhanceCostMultiplier;
    private float sellPriceMultiplier;
    private float customerSpawnRate;

    // 보너스 스탯
    private float bonusCraftSpeedMultiplier;
    private float bonusRareItemChance;
    private float bonusEnhanceSuccessRate;
    private float bonusBreakChanceReduction;
    private float bonusEnhanceCostMultiplier;
    private float bonusSellPriceMultiplier;
    private float bonusCustomerSpawnRate;

    // 최종 스탯
    public float FinalCraftSpeedMultiplier => craftSpeedMultiplier + bonusCraftSpeedMultiplier;
    public float FinalRareItemChance => rareItemChance + bonusRareItemChance;
    public float FinalEnhanceSuccessRate => enhanceSuccessRate + bonusEnhanceSuccessRate;
    public float FinalBreakChanceReduction => breakChanceReduction + bonusBreakChanceReduction;
    public float FinalEnhanceCostMultiplier => enhanceCostMultiplier + bonusEnhanceCostMultiplier;
    public float FinalSellPriceMultiplier => sellPriceMultiplier + bonusSellPriceMultiplier;
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
    public Dictionary<SpecializationType, AssistantInstance> EquippedAssistant;

    [Header("Player")]
    [SerializeField] BlackSmith blackSmith;

    [Header("Assitant Roots")]
    [SerializeField] Transform craftingAssitantRoot;
    [SerializeField] Transform enhanceAssistantRoot;
    [SerializeField] Transform sellingAssitantRoot;

    public BlackSmith BlackSmith { get => blackSmith; }
    public WeaponSellingSystem SellingSystem { get; private set; }

    // 이벤트 핸들러
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        InitAssistant();

        SellingSystem = GetComponent<WeaponSellingSystem>();

        if (SellingSystem)
            SellingSystem.Init(this, gameManager.DataManager);

        if (blackSmith != null)
            blackSmith.Init();
    }

    private void InitAssistant()
    {
        EquippedAssistant = new Dictionary<SpecializationType, AssistantInstance>();

        EquippedAssistant[SpecializationType.Crafting] = null;
        EquippedAssistant[SpecializationType.Enhancing] = null;
        EquippedAssistant[SpecializationType.Selling] = null;
    }

    private void RaiseAllEvents()
    {
        Events.RaiseGoldChanged(Gold);
        Events.RaiseDiaChanged(Dia);
        Events.RaiseFameChanged(CurrentFame, MaxFame);
        Events.RaiseLevelChanged(Level);
        Events.RasieTotalFameChanged(TotalFame);
    }

    public void LoadData(ForgeData data)
    {
        // 제작 관련 스탯
        craftSpeedMultiplier = data.CraftSpeedMultiplier;
        rareItemChance = data.RareItemChance;

        // 강화 관련 스탯
        enhanceSuccessRate = data.EnhanceSuccessRate;
        breakChanceReduction = data.BreakChanceReduction;
        enhanceCostMultiplier = data.EnhanceCostMultiplier;

        // 판매 관련 스탯
        sellPriceMultiplier = data.SellPriceMultiplier;
        customerSpawnRate = data.CustomerSpawnRate;

        Level = data.Level;
        CurrentFame = data.CurrentFame;
        MaxFame = data.MaxFame;
        TotalFame = data.TotalFame;

        Gold = data.Gold;
        Dia = data.Dia;

        RaiseAllEvents();
    }

    public ForgeData SaveData()
    {
        ForgeData data = new ForgeData
        {
            CraftSpeedMultiplier = craftSpeedMultiplier,
            RareItemChance = rareItemChance,

            EnhanceSuccessRate = enhanceSuccessRate,
            BreakChanceReduction = breakChanceReduction,
            EnhanceCostMultiplier = enhanceCostMultiplier,

            SellPriceMultiplier = sellPriceMultiplier,
            CustomerSpawnRate = customerSpawnRate,

            Level = Level,
            MaxFame = MaxFame,
            CurrentFame = CurrentFame,
            TotalFame = TotalFame,
            Gold = Gold,
            Dia = Dia
        };

        return data;
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
    }

    public void DeActiveAssistant(AssistantInstance assi)
    {
        assi.IsEquipped = false;
        EquippedAssistant[assi.Specialization] = null;
        Events.RaiseAssistantChanged(assi, false);

        DeApplyAssistantStat(assi);
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

                case AssistantStatNames.IncreaseEnhanceChance:
                    bonusEnhanceSuccessRate += stat.Multiplier;
                    break;

                case AssistantStatNames.DecreaseBreakChance:
                    bonusBreakChanceReduction += stat.Multiplier;
                    break;

                case AssistantStatNames.DecreaseEnhanceCost:
                    bonusEnhanceCostMultiplier += stat.Multiplier;
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

                case AssistantStatNames.IncreaseEnhanceChance:
                    bonusEnhanceSuccessRate -= stat.Multiplier;
                    break;

                case AssistantStatNames.DecreaseBreakChance:
                    bonusBreakChanceReduction -= stat.Multiplier;
                    break;

                case AssistantStatNames.DecreaseEnhanceCost:
                    bonusEnhanceCostMultiplier -= stat.Multiplier;
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
}
