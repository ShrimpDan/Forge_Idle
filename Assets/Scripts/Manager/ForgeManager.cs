using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    private GameManager gameManager;
    private UIManager uIManager;
    private AssistantInventory assistantInventory;

    // 대장간 이름
    public string Name { get; private set; }

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

    public List<ForgeType> UnlockedForge { get; private set; } = new();
    public Forge CurrentForge { get; private set; }
    public ForgeTypeSaveSystem ForgeTypeSaveSystem { get; private set; } = new ForgeTypeSaveSystem();
    public ForgeSkillSystem SkillSystem { get; private set; }
    public ForgeEventHandler Events { get; private set; } = new ForgeEventHandler();
    public Dictionary<ForgeType, Dictionary<SpecializationType, AssistantInstance>> EquippedAssistant { get; private set; } = new();

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        uIManager = gameManager.UIManager;
        assistantInventory = gameManager.AssistantInventory;

        SkillSystem = GetComponentInChildren<ForgeSkillSystem>();

        if (SkillSystem)
            SkillSystem.Init(this, gameManager.SkillManager);
    }

    public ForgeCommonData SaveToData()
    {
        var data = new ForgeCommonData
        {
            Name = Name,
            Level = Level,
            CurrentFame = CurrentFame,
            MaxFame = MaxFame,
            TotalFame = TotalFame,

            CurRecipePoint = CurRecipePoint,
            TotalRecipePoint = TotalRecipePoint,

            Gold = Gold,
            Dia = Dia,

            ActiveSkills = SkillSystem.GetSaveData(),
            EquippedAssi = GetAssiSaveData(),

            UnlockedForge = UnlockedForge,
            CurrentForgeScene = CurrentForge != null ? CurrentForge.SceneType : SceneType.Forge_Weapon
        };

        if (CurrentForge != null)
            ForgeTypeSaveSystem.SaveForgeType(CurrentForge);

        return data;
    }

    public void LoadFromData(ForgeCommonData data)
    {
        Name = data.Name;
        Level = data.Level;
        CurrentFame = data.CurrentFame;
        MaxFame = data.MaxFame;
        TotalFame = data.TotalFame;

        CurRecipePoint = data.CurRecipePoint;
        TotalRecipePoint = data.TotalRecipePoint;

        Gold = data.Gold;
        Dia = data.Dia;

        SkillSystem.LoadFromData(data.ActiveSkills);
        LoadAssiSaveData(data.EquippedAssi);

        UnlockedForge = data.UnlockedForge;
        LoadSceneManager.Instance.LoadSceneAsync(data.CurrentForgeScene, true);

        RaiseAllEvents();
    }

    private void RaiseAllEvents()
    {
        Events.RaiseNameChanged(Name);
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

        if (string.IsNullOrEmpty(Name))
        {
            uIManager.OpenUI<NickNameWindow>(UIName.NickNameWindow);
        }
    }

    public void UnlockForge(ForgeType forgeType)
    {
        if (forgeType == ForgeType.None) return;
        
        if (!UnlockedForge.Contains(forgeType))
        {
            UnlockedForge.Add(forgeType);
        }
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

            AddPoint(5);
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

        var ui = uIManager.OpenUI<LackPopup>(UIName.LackPopup);
        ui.Show(LackType.Gold);
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

        var ui = uIManager.OpenUI<LackPopup>(UIName.LackPopup);
        ui.Show(LackType.Dia);
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
            var ui = uIManager.OpenUI<LackPopup>(UIName.LackPopup);
            ui.Show(LackType.Point);
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

    public void SetNickName(string nickName)
    {
        Name = nickName;
        Events.RaiseNameChanged(Name);
    }

    public AssistantInstance GetEquippedAssi(string key)
    {
        return EquippedAssistant.SelectMany(forgeEntry => forgeEntry.Value.Values)
                                .FirstOrDefault(assistant => assistant.Key == key);
    }

    private List<EquipAssiSaveData> GetAssiSaveData()
    {
        List<EquipAssiSaveData> equippedAssi = new List<EquipAssiSaveData>();

        foreach (var forgeType in EquippedAssistant.Keys)
        {
            foreach (var assi in EquippedAssistant[forgeType].Values)
            {
                if (assi != null)
                {
                    EquipAssiSaveData data = new EquipAssiSaveData
                    {
                        ForgeType = forgeType,
                        AssistantKey = assi.Key
                    };

                    equippedAssi.Add(data);
                }
            }
        }

        return equippedAssi;
    }

    private void LoadAssiSaveData(List<EquipAssiSaveData> equippedAssi)
    {
        EquippedAssistant = new Dictionary<ForgeType, Dictionary<SpecializationType, AssistantInstance>>()
        {
            { ForgeType.Weapon, new Dictionary<SpecializationType, AssistantInstance>()
                {
                    { SpecializationType.Crafting, null },
                    { SpecializationType.Selling, null }
                }
            },
            { ForgeType.Armor, new Dictionary<SpecializationType, AssistantInstance>()
                {
                    { SpecializationType.Crafting, null },
                    { SpecializationType.Selling, null }
                }
            },
            { ForgeType.Magic, new Dictionary<SpecializationType, AssistantInstance>()
                {
                    { SpecializationType.Crafting, null },
                    { SpecializationType.Selling, null }
                }
            }
        };

        if (equippedAssi != null)
        {
            foreach (var data in equippedAssi)
            {
                AssistantInstance assi = assistantInventory.GetAssistantInstance(data.AssistantKey);

                if (EquippedAssistant.ContainsKey(data.ForgeType))
                    EquippedAssistant[data.ForgeType][assi.Specialization] = assi;
            }
        }
    }
}
