using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.Requests;
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
    public string Name;
    public int Level;
    public string PersonalityKey;
    public SpecializationType Specialization;
    public List<AbilityMultiplierSave> Multipliers;
    public bool IsEquipped;
    public bool IsInUse;
    public int SpecializationIndex;
}

public class AssistantSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "assistant_save.json");

    public static void SaveAssistant(TraineeInventory inventory)
    {
        var saveData = new AssistantSaveData
        {
            Assistants = new List<AssistantDataSave>()
        };

        foreach (var assi in inventory.GetAll())
        {
            var a = new AssistantDataSave
            {
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
                SpecializationIndex = assi.SpecializationIndex
            };

            saveData.Assistants.Add(a);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("[저장 시스템] 제자 데이터 저장 완료");
    }

    public static void LoadAssistants(TraineeInventory inventory, PersonalityDataLoader personalityLoader)
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
            var multipliers = a.Multipliers.ConvertAll(a => new TraineeData.AbilityMultiplier(a.AbilityName, a.Multiplier));

            var assi = new TraineeData(a.Name, personality, a.Specialization, multipliers, a.Level, a.IsEquipped, a.IsInUse);
            assi.SpecializationIndex = a.SpecializationIndex;

            inventory.Add(assi);
        }

        Debug.Log("[저장 시스템] 제자 데이터 불러오기 완료");
    }
}

public class AssistantSaveHandler : ISaveHandler
{
    private TraineeManager traineeManager;
    private PersonalityDataLoader personalityLoader;

    public AssistantSaveHandler(TraineeManager manager, PersonalityDataLoader loader)
    {
        traineeManager = manager;
        personalityLoader = loader;
    }

    public void Save()
    {
        AssistantSaveSystem.SaveAssistant(traineeManager.TraineeInventory);
    }

    public void Load()
    {
        AssistantSaveSystem.LoadAssistants(traineeManager.TraineeInventory, personalityLoader);
    }
}
