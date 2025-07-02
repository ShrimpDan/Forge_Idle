using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EffectDatabaseAutoLoader
{
    private static readonly string[] folderOrder = new[]
    {
        "Ambient",
        "Combat",
        "Economy",
        "Trainee",
        "Production",
        "UI"
    };

    private const string rootPath = "Assets/ScriptableObjects/Effect";
    private const string dbPath = rootPath + "/_DataBase/EffectDatabase.asset";

    [MenuItem("Tools/Effect/Build Ordered EffectDatabase")]
    public static void BuildOrderedDatabase()
    {
        var database = AssetDatabase.LoadAssetAtPath<EffectDatabase>(dbPath);
        if (database == null)
        {
            Debug.LogError("EffectDatabase.asset 경로를 확인해주세요 : " + dbPath);
            return;
        }

        List<EffectData> resultList = new();

        foreach (var folder in folderOrder)
        {
            string fullPath = $"{rootPath}/{folder}";
            string[] guids = AssetDatabase.FindAssets("t:EffectData", new[] { fullPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EffectData data = AssetDatabase.LoadAssetAtPath<EffectData>(path);

                if (data != null && !resultList.Contains(data))
                {
                    resultList.Add(data);
                }
            }
        }

        database.effects.Clear();
        database.effects.AddRange(resultList);

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"EffectDatabase 등록 완료, 총 {resultList.Count}개 정렬됨.");
    }
}
