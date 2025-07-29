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
    }

    public static void LoadDungeonSystem(DungeonSystem dungeonSystem)
    {
        if (!File.Exists(SavePath))
        {
            dungeonSystem.LoadFromSaveData(null);
            return;
        }

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<DungeonSaveData>(json);
        dungeonSystem.LoadFromSaveData(saveData);
    }

    public static void Delete(DungeonSystem dungeonSystem)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            dungeonSystem.ClearUnlockDungeon();
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
