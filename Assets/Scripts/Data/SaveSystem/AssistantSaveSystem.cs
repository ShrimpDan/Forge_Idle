using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AssistantSaveData
{
    public List<AssistantDataSave> Assistants;
}

[System.Serializable]
public class AbilityMultiplierSave
{
    public string AbilityName;
    public float Multiplier;
}

[System.Serializable]
public class AssistantDataSave
{
    public string Key;
    public string Name;
    public int Level;
    public string PersonalityKey;
    public SpecializationType Specialization;
    public List<AbilityMultiplierSave> Multipliers;
    public ForgeType EquippedForge;
    public bool IsEquipped;
    public bool IsInUse;
    public int SpecializationIndex;
    public string IconPath;

    public int Wage;
    public int RecruitCost;
    public int RehireCost;
    public bool IsFired;

    public string grade;
    public string customerInfo;
}

public class AssistantSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "assistant_save.json");

    public static void SaveAssistant(AssistantInventory inventory)
    {
        var saveData = new AssistantSaveData
        {
            Assistants = new List<AssistantDataSave>()
        };

        foreach (var assi in inventory.GetAll())
        {
            if (assi == null)
            {
                Debug.LogWarning("[AssistantSaveSystem] Null 어시스턴트가 인벤토리에 있습니다. 저장에서 제외합니다.");
                continue;
            }
            if (assi.Personality == null)
            {
                Debug.LogWarning($"[AssistantSaveSystem] Personality==null! 저장 불가. Key={assi.Key}, Name={assi.Name}");
                continue;
            }

            var a = new AssistantDataSave
            {
                Key = assi.Key,
                Name = assi.Name,
                Level = assi.Level,
                PersonalityKey = assi.Personality.Key,
                Specialization = assi.Specialization,
                Multipliers = assi.Multipliers.ConvertAll(m => new AbilityMultiplierSave
                {
                    AbilityName = m.AbilityName,
                    Multiplier = m.Multiplier
                }),
                EquippedForge = assi.EquippedForge,
                IsEquipped = assi.IsEquipped,
                IsInUse = assi.IsInUse,
                SpecializationIndex = assi.SpecializationIndex,
                IconPath = assi.IconPath,
                Wage = assi.Wage,
                RecruitCost = assi.RecruitCost,
                RehireCost = assi.RehireCost,
                IsFired = assi.IsFired,

                grade = assi.grade,
                customerInfo = assi.CustomerInfo
            };

            saveData.Assistants.Add(a);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadAssistants(AssistantInventory inventory, PersonalityDataLoader personalityLoader)
    {
        if (!File.Exists(SavePath))
            return;

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<AssistantSaveData>(json);

        inventory.GetAll().Clear();

        foreach (var a in saveData.Assistants)
        {
            var personality = personalityLoader.GetByKey(a.PersonalityKey);
            var multipliers = a.Multipliers.ConvertAll(m => new AssistantInstance.AbilityMultiplier(m.AbilityName, m.Multiplier));

            var assi = new AssistantInstance(
                key: a.Key,
                name: a.Name,
                personality: personality,
                specialization: a.Specialization,
                multipliers: multipliers,
                iconPath: a.IconPath,
                level: a.Level,
                isEquipped: a.IsEquipped,
                isInuse: a.IsInUse,
                grade: a.grade,
                customerInfo: a.customerInfo,
                recruitCost: a.RecruitCost,
                wage: a.Wage,
                rehireCost: a.RehireCost,
                forgeType: a.EquippedForge
            );

            assi.SpecializationIndex = a.SpecializationIndex;
            assi.IsFired = a.IsFired;

            inventory.Add(assi);
        }
    }

    public static void Delete(AssistantInventory inventory)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            inventory.Clear();
        }
    }
}

public class AssistantSaveHandler : ISaveHandler
{
    private AssistantManager assistantManager;
    private PersonalityDataLoader personalityLoader;

    public AssistantSaveHandler(AssistantManager manager, PersonalityDataLoader loader)
    {
        assistantManager = manager;
        personalityLoader = loader;
    }

    public void Save()
    {
        AssistantSaveSystem.SaveAssistant(assistantManager.AssistantInventory);
    }

    public void Load()
    {
        AssistantSaveSystem.LoadAssistants(assistantManager.AssistantInventory, personalityLoader);
    }

    public void Delete()
    {
        AssistantSaveSystem.Delete(assistantManager.AssistantInventory);
    }
}
