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
    public bool IsEquipped;
    public bool IsInUse;
    public int SpecializationIndex;
    public string IconPath;
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
            var a = new AssistantDataSave
            {
                Key = assi.Key,
                Name = assi.Name,
                Level = assi.Level,
                PersonalityKey = assi.Personality.Key,
                Specialization = assi.Specialization,
                Multipliers = assi.Multipliers.ConvertAll(a => new AbilityMultiplierSave
                {
                    AbilityName = a.AbilityName,
                    Multiplier = a.Multiplier
                }),
                IsEquipped = assi.IsEquipped,
                IsInUse = assi.IsInUse,
                SpecializationIndex = assi.SpecializationIndex,
                IconPath = assi.IconPath
            };

            saveData.Assistants.Add(a);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[저장 시스템] 제자 데이터 저장 완료 \n경로: {SavePath}");
    }

    public static void LoadAssistants(AssistantInventory inventory, PersonalityDataLoader personalityLoader)
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[저장 시스템] 제자 데이터가 존재하지않습니다.");
            return;
        }

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
                isInuse: a.IsInUse
            );
            assi.SpecializationIndex = a.SpecializationIndex;

            inventory.Add(assi);
        }

        Debug.Log("[저장 시스템] 제자 데이터 불러오기 완료");
    }

    public static void Delete(AssistantInventory inventory)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            inventory.Clear();
            Debug.Log("Assistant 삭제");
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
