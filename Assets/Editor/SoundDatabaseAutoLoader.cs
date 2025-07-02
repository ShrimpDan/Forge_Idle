using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SoundDatabaseAutoLoader
{
    private static readonly string[] folderOrder = new[]
    {
        "BGM",
        "SFX"
    };

    private const string rootPath = "Assets/ScriptableObjects/Sound";
    private const string dbPath = rootPath + "/_DataBase/SoundDataBase.asset";

    [MenuItem("Tools/Sound/Build Ordered SoundDatabase")]
    public static void BuildOrderedDatabase()
    {
        var database = AssetDatabase.LoadAssetAtPath<SoundDatabase>(dbPath);
        if (database == null)
        {
            Debug.LogError("SoundDataBase.asset 경로를 확인해주세요 : " + dbPath);
            return;
        }

        List<SoundData> resultList = new();

        foreach (var folder in folderOrder)
        {
            string fullPath = $"{rootPath}/{folder}";
            string[] guids = AssetDatabase.FindAssets("t:SoundData", new[] { fullPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SoundData data = AssetDatabase.LoadAssetAtPath<SoundData>(path);

                if (data != null && !resultList.Contains(data))
                {
                    resultList.Add(data);
                }
            }
        }

        database.sounds.Clear();
        database.sounds.AddRange(resultList);

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"SoundDatabase 등록 완료, 총 {resultList.Count}개 정렬됨.");
    }
}
