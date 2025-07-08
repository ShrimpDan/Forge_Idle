using System.Collections.Generic;
using UnityEngine;

public static class QuestProgressManager
{
    public static Dictionary<string, QuestProgressData> LoadAll()
    {
        var dict = new Dictionary<string, QuestProgressData>();
        var allKeys = PlayerPrefs.GetString("AllQuestProgressKeys", "");
        if (string.IsNullOrEmpty(allKeys)) return dict;

        foreach (var key in allKeys.Split(','))
        {
            string json = PlayerPrefs.GetString($"quest_progress_{key}", "");
            if (!string.IsNullOrEmpty(json))
                dict[key] = JsonUtility.FromJson<QuestProgressData>(json);
        }
        return dict;
    }

    public static void Save(string questKey, QuestProgressData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString($"quest_progress_{questKey}", json);

        // 전체 키 목록 갱신
        var allKeys = PlayerPrefs.GetString("AllQuestProgressKeys", "");
        if (!allKeys.Contains(questKey))
        {
            if (!string.IsNullOrEmpty(allKeys))
                allKeys += ",";
            allKeys += questKey;
            PlayerPrefs.SetString("AllQuestProgressKeys", allKeys);
        }
        PlayerPrefs.Save();
    }

    public static QuestProgressData Load(string questKey)
    {
        string json = PlayerPrefs.GetString($"quest_progress_{questKey}", "");
        return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<QuestProgressData>(json) : null;
    }

    public static void Delete(string questKey)
    {
        PlayerPrefs.DeleteKey($"quest_progress_{questKey}");
        var allKeys = PlayerPrefs.GetString("AllQuestProgressKeys", "");
        var keyList = new List<string>(allKeys.Split(','));
        keyList.Remove(questKey);
        PlayerPrefs.SetString("AllQuestProgressKeys", string.Join(",", keyList));
        PlayerPrefs.Save();
    }
}
