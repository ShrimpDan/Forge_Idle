using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ActiveSkillSaveData
{
    public string SkillKey;
    public int Idx;
}

[System.Serializable]
public class EquipAssiSaveData
{
    public ForgeType? ForgeType;
    public string AssistantKey; 
}

[System.Serializable]
public class ForgeCommonData
{
    // 레벨 & 명성치
    public int Level;
    public int CurrentFame;
    public int MaxFame;
    public int TotalFame;

    // 레시피 포인트
    public int CurRecipePoint;
    public int TotalRecipePoint;

    // 골드 & 다이아
    public int Gold;
    public int Dia;

    // 스킬
    public List<ActiveSkillSaveData> ActiveSkills;
    public List<EquipAssiSaveData> EquippedAssi;

    public SceneType CurrentForgeScene;
}

public static class ForgeSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "forge_common.json");

    public static void SaveForge(ForgeManager manager)
    {
        var data = manager.SaveToData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[저장 시스템] {manager} 저장이 완료되었습니다.\n 경로: {SavePath}");
    }

    public static void LoadForge(ForgeManager manager)
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning($"[저장 시스템] {manager}의 데이터가 존재하지 않습니다.");

            var newData = GetDefaultData();
            manager.LoadFromData(newData);
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<ForgeCommonData>(json.ToString());
        manager.LoadFromData(data);
    }

    public static void Delete(ForgeManager manager)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);

            var newData = GetDefaultData();
            manager.LoadFromData(newData);

            Debug.Log($"{manager} 데이터 삭제");
        }
    }

    private static ForgeCommonData GetDefaultData()
    {
        return new ForgeCommonData
        {
            Level = 1,
            CurrentFame = 0,
            MaxFame = 100,
            TotalFame = 0,

            CurRecipePoint = 0,
            TotalRecipePoint = 0,
            
            Gold = 0,
            Dia = 0,

            ActiveSkills = new List<ActiveSkillSaveData>(),
            EquippedAssi = new List<EquipAssiSaveData>(),

            CurrentForgeScene = SceneType.Forge_Weapon
        };
    }
}

public class ForgeSaveHandeler : ISaveHandler
{
    private ForgeManager forgeManager;

    public ForgeSaveHandeler(ForgeManager forgeManager)
    {
        this.forgeManager = forgeManager;
    }

    public void Save()
    {
        ForgeSaveSystem.SaveForge(forgeManager);
    }

    public void Load()
    {
        ForgeSaveSystem.LoadForge(forgeManager);
    }

    public void Delete()
    {
        ForgeSaveSystem.Delete(forgeManager);
    }
}
