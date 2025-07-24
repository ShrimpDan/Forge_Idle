using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SkillSaveData
{
    public string Key;
    public int Level;
    public int CurCount;
    public int NeedCount;
}

public class SkillSaveDataList
{
    public List<SkillSaveData> savedSkills;
}

public class SkillSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "skill_save.json");

    public static void SaveSkill(SkillManager skillManager)
    {
        var saveData = skillManager.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadSkill(SkillManager skillManager)
    {
        if (!File.Exists(SavePath))
        {
            skillManager.ClearSkill();
            return;
        }

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<SkillSaveDataList>(json);
        skillManager.LoadFromSaveData(saveData);
    }

    public static void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}

public class SkillSaveHandler : ISaveHandler
{
    private SkillManager skillManager;

    public SkillSaveHandler(SkillManager skillManager)
    {
        this.skillManager = skillManager;
    }

    public void Save()
    {
        SkillSaveSystem.SaveSkill(skillManager);
    }

    public void Load()
    {
        SkillSaveSystem.LoadSkill(skillManager);
    }

    public void Delete()
    {
        SkillSaveSystem.Delete();
    }
}
