using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CollectionBookSaveData
{
    public List<string> discoveredKeys;
}

public class CollectionBookSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "collectionbook_save.json");

    public static void SaveCollection(CollectionBookManager manager)
    {
        var saveData = manager.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadCollection(CollectionBookManager manager)
    {
        if (!File.Exists(SavePath))
        {
            return;
        }

        var json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<CollectionBookSaveData>(json);
        manager.LoadFromSaveData(saveData);
    }

    public static void Delete(CollectionBookManager manager)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            manager.ClearCollectionBook();
        }
    }
}

public class CollectionBookSaveHandler : ISaveHandler
{
    private CollectionBookManager collectionBookManager;

    public CollectionBookSaveHandler(CollectionBookManager manager)
    {
        collectionBookManager = manager;
    }

    public void Save()
    {
        CollectionBookSaveSystem.SaveCollection(collectionBookManager);
    }

    public void Load()
    {
        CollectionBookSaveSystem.LoadCollection(collectionBookManager);
    }

    public void Delete()
    {
        CollectionBookSaveSystem.Delete(collectionBookManager);
    }
}
