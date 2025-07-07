using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CollectionBookSaveData
{

}

public class CollectionBookSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "collectionbook_save.json");

    public static void SaveInventory(CollectionBookManager manager)
    {
        var saveData = manager.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[저장 시스템] 컬렉션 북 저장이 완료되었습니다.\n 경로: {SavePath}");
    }
}
