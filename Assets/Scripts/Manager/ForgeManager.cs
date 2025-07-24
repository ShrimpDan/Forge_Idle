using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    private GameManager gameManager;

    // 레벨 & 명성치
    public int Level { get; private set; }
    public int CurrentFame { get; private set; }
    public int MaxFame { get; private set; }
    public int TotalFame { get; private set; }

    // 레시피 포인트
    public int CurRecipePoint { get; private set; }
    public int TotalRecipePoint { get; private set; }
    private int UsedPoint => TotalRecipePoint - CurRecipePoint;
    private int resetGold;

    // 골드 & 다이아
    public int Gold { get; private set; }
    public int Dia { get; private set; }

    public Forge CurrentForge { get; private set; }

    public ForgeTypeSaveSystem ForgeTypeSaveSystem { get; private set; } = new ForgeTypeSaveSystem();
    public ForgeSkillSystem SkillSystem { get; private set; }
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        SkillSystem = GetComponentInChildren<ForgeSkillSystem>();
    }

    public ForgeCommonData SaveToData()
    {
        var data = new ForgeCommonData
        {
            Level = Level,
            CurrentFame = CurrentFame,
            MaxFame = MaxFame,
            TotalFame = TotalFame,

            CurRecipePoint = CurRecipePoint,
            TotalRecipePoint = TotalRecipePoint,

            Gold = Gold,
            Dia = Dia,
            CurrentForgeScene = CurrentForge != null ? CurrentForge.SceneType : SceneType.Main
        };

        if (CurrentForge != null)
            ForgeTypeSaveSystem.SaveForgeType(CurrentForge);

        return data;
    }

    public void LoadFromData(ForgeCommonData data)
    {
        Level = data.Level;
        CurrentFame = data.CurrentFame;
        MaxFame = data.MaxFame;
        TotalFame = data.TotalFame;

        CurRecipePoint = data.CurRecipePoint;
        TotalRecipePoint = data.TotalRecipePoint;

        Gold = data.Gold;
        Dia = data.Dia;

        if (data.CurrentForgeScene != SceneType.Main)
        {
            LoadSceneManager.Instance.LoadSceneAsync(data.CurrentForgeScene, true);
        }

        RaiseAllEvents();
    }


    private void RaiseAllEvents()
    {
        Events.RaiseGoldChanged(Gold);
        Events.RaiseDiaChanged(Dia);
        Events.RaiseFameChanged(CurrentFame, MaxFame);
        Events.RaiseLevelChanged(Level);
        Events.RaiseTotalFameChanged(TotalFame);
    }

    public void SetCurrentForge(Forge forge)
    {
        if (CurrentForge != null)
        {
            CurrentForge.ExitForge();
        }

        CurrentForge = forge;
        gameManager.UIManager.OpenForgeTab();
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
        Events.RaiseTotalFameChanged(TotalFame);
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

    public void AddPoint(int amount)
    {
        CurRecipePoint += amount;
        TotalRecipePoint += amount;
    }

    public bool UsePoint(int amount)
    {
        if (CurRecipePoint - amount < 0)
        {
            return false;
        }

        CurRecipePoint -= amount;
        return true;
    }

    public void ResetPoint()
    {
        if (UseGold(resetGold * UsedPoint))
        {
            CurRecipePoint = TotalRecipePoint;
            CurrentForge.RecipeSystem.ResetRecipe();
        }
    }
}
