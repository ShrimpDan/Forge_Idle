using System.Collections.Generic;
using UnityEngine;

public class Forge : MonoBehaviour
{
    private ForgeData forgeData;

    // 스탯
    public float CraftTimeMultiplier { get; private set; }
    public float SellPriceMultiplier { get; private set; }
    public float RareItemChance { get; private set; }
    public float CustomerSpawnDelay { get; private set; }

    // 보너스 스탯
    public float BonusCraftTimeMultiplier { get; private set; }
    public float BonusSellPriceMultiplier { get; private set; }
    public float BonusRareItemChance { get; private set; }
    public float BonusCustomerSpawnDelay { get; private set; }

    // 최종 스탯
    public float FinalCraftTimeMulitiplier => CraftTimeMultiplier + BonusCraftTimeMultiplier;
    public float FinalSellPriceMultiplier => SellPriceMultiplier + BonusSellPriceMultiplier;
    public float FinalRareItemChance => RareItemChance + BonusRareItemChance;
    public float FinalCustomerSpawnDelay => CustomerSpawnDelay - BonusCustomerSpawnDelay;

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

        CraftTimeMultiplier = forgeData.CraftTimeMultiplier;
        SellPriceMultiplier = forgeData.SellPriceMultiplier;
        RareItemChance = forgeData.RareItemChance;
        CustomerSpawnDelay = forgeData.CustomerSpawnDelay;

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
            CraftTimeMultiplier = CraftTimeMultiplier,
            SellPriceMultiplier = SellPriceMultiplier,
            RareItemChance = RareItemChance,
            CustomerSpawnDelay = CustomerSpawnDelay,
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
        EquippedAssistant[assi.Specialization] = assi;
        Events.RaiseAssistantChanged(assi, true);
    }

    public void DeActiveAssistant(TraineeData assi)
    {
        EquippedAssistant[assi.Specialization] = null;
        Events.RaiseAssistantChanged(assi, false);
    }
}
