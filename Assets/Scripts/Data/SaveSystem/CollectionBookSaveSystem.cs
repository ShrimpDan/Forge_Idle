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
        Debug.Log($"[저장 시스템] 컬렉션 북 저장이 완료되었습니다.\n 경로: {SavePath}");
    }

    public static void LoadCollection(CollectionBookManager manager)
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[저장 시스템] 세이브 파일이 존재하지않습니다.");
            return;
        }

        var json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<CollectionBookSaveData>(json);
        manager.LoadFromSaveData(saveData);
        Debug.Log("[저장 시스템] 컬렉션 북 로드 완료.");
    }

    public static void Delete(CollectionBookManager manager)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            manager.ClearCollectionBook();
            Debug.Log("CollectionBookSaveData 삭제");
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
