using System.Collections.Generic;
using UnityEngine;

public class Forge : MonoBehaviour
{
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
    public Dictionary<SpecializationType, TraineeData> EquippedAssistant;
    // 이벤트 핸들러
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();

    public void Init()
    {
        InitData();
        InitAssistant();
    }

    private void InitData()
    {
        forgeData = ForgeDataSaveLoader.Load();

        // 제작 관련 스탯
        craftSpeedMultiplier = forgeData.CraftSpeedMultiplier;
        rareItemChance = forgeData.RareItemChance;

        // 강화 관련 스탯
        enhanceSuccessRate = forgeData.EnhanceSuccessRate;
        breakChanceReduction = forgeData.BreakChanceReduction;
        enhanceCostMultiplier = forgeData.EnhanceCostMultiplier;

        // 판매 관련 스탯
        sellPriceMultiplier = forgeData.SellPriceMultiplier;
        customerSpawnRate = forgeData.CustomerSpawnRate;

        Level = forgeData.Level;
        CurrentFame = forgeData.CurrentFame;
        MaxFame = forgeData.MaxFame;
        TotalFame = forgeData.TotalFame;

        Gold = forgeData.Gold;
        Dia = forgeData.Dia;

        RaiseAllEvents();
    }

    private void InitAssistant()
    {
        EquippedAssistant = new Dictionary<SpecializationType, TraineeData>();

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

    public void SaveData()
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

        ForgeDataSaveLoader.Save(data);
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

    public void ActiveAssistant(TraineeData assi)
    {
        // 이전에 등록된 제자가 있다면 해제
        TraineeData preAssi = EquippedAssistant[assi.Specialization];
        if (preAssi != null)
        {
            preAssi.IsEquipped = false;
            Events.RaiseAssistantChanged(preAssi, false);
        }

        EquippedAssistant[assi.Specialization] = assi;
        Events.RaiseAssistantChanged(assi, true);
        assi.IsEquipped = true;
    }

    public void DeActiveAssistant(TraineeData assi)
    {
        assi.IsEquipped = false;
        EquippedAssistant[assi.Specialization] = null;
        Events.RaiseAssistantChanged(assi, false);
    }

    public void ApplyAssistantStat(TraineeData assi)
    {
        foreach (var stat in assi.Multipliers)
        {
            switch (stat.AbilityName)
            {
                case TraineeStatNames.IncreaseCraftSpeed:
                    break;

                case TraineeStatNames.IncreaseAdvancedCraftChance:
                    break;

                case TraineeStatNames.IncreaseEnhanceChance:
                    break;

                case TraineeStatNames.DecreaseBreakChance:
                    break;

                case TraineeStatNames.DecreaseEnhanceCost:
                    break;

                case TraineeStatNames.IncreaseSellPrice:
                    break;

                case TraineeStatNames.IncreaseCustomerCount:
                    break;
            }
        }
    }
}
