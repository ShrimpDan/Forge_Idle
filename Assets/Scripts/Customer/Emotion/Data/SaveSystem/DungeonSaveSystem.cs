using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DungeonSaveData
{
    public List<string> dungeonKeys;
}

public class DungeonSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "dungeon_save.json");

    public static void SaveDungeonSystem(DungeonSystem dungeonSystem)
    {
        var saveData = dungeonSystem.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[저장 시스템] 던전시스템 저장이 완료되었습니다.\n 경로: {SavePath}");
    }

    public static void LoadDungeonSystem(DungeonSystem dungeonSystem)
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[저장 시스템] 세이브 파일이 존재하지않습니다.");
            dungeonSystem.LoadFromSaveData(null);
            return;
        }

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<DungeonSaveData>(json);
        dungeonSystem.LoadFromSaveData(saveData);

        Debug.Log("[저장 시스템] 던전시스템 로드 완료.");
    }

    public static void Delete(DungeonSystem dungeonSystem)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            dungeonSystem.ClearUnlockDungeon();
            Debug.Log("DungeonSaveData 삭제");
        }
    }
}

public class DungeonSaveHandler : ISaveHandler
{
    private DungeonSystem dungeonSystem;

    public DungeonSaveHandler(DungeonSystem system)
    {
        dungeonSystem = system;
    }

    public void Save()
    {
        DungeonSaveSystem.SaveDungeonSystem(dungeonSystem);
    }

    public void Load()
    {
        DungeonSaveSystem.LoadDungeonSystem(dungeonSystem);
    }

    public void Delete()
    {
        DungeonSaveSystem.Delete(dungeonSystem);
    }
}
