using System.Collections.Generic;
using System.Linq;

public class ForgeManager
{
    private GameManager gameManager;

    // 레벨 & 명성치
    public int Level { get; private set; }
    public int CurrentFame { get; private set; }
    public int MaxFame { get; private set; }
    public int TotalFame { get; private set; }

    // 골드 & 다이아
    public int Gold { get; private set; }
    public int Dia { get; private set; }

    public ForgeType CurrentForgeType { get; private set; }

    private ForgeTypeSaveSystem forgeTypeSaveSystem = new ForgeTypeSaveSystem();
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();

    public ForgeManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public ForgeCommonData SaveToData()
    {
        var data = new ForgeCommonData
        {
            Level = Level,
            CurrentFame = CurrentFame,
            MaxFame = MaxFame,
            TotalFame = TotalFame,

            Gold = Gold,
            Dia = Dia,

            CurrentForgeType = CurrentForgeType
        };

        return data;
    }

    public void LoadFromData(ForgeCommonData data)
    {
        Level = data.Level;
        CurrentFame = data.CurrentFame;
        MaxFame = data.MaxFame;
        TotalFame = data.TotalFame;

        Gold = data.Gold;
        Dia = data.Dia;

        CurrentForgeType = CurrentForgeType;
    }

    public void SetCurrentForge(ForgeType type)
    {
        CurrentForgeType = type;
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
}
