using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string Key;
    public string QuestName;
    public int Difficulty;
    public int Duration;           
    public int RequiredAssistants; 
    public int RewardMin;
    public int RewardMax;
    public List<string> RewardItemKeys;
}

public class QuestLoader
{
    public List<QuestData> QuestLists { get; private set; }
    public Dictionary<string, QuestData> QuestDict { get; private set; }

    public QuestLoader(string path = "Data/quest_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("의뢰 데이터가 존재하지 않습니다.");
            return;
        }

        QuestLists = JsonUtility.FromJson<Wrapper>(json.text).Items;

        QuestDict = new Dictionary<string, QuestData>();
        foreach (var quest in QuestLists)
        {
            QuestDict[quest.Key] = quest;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<QuestData> Items;
    }

    public QuestData GetQuestByKey(string key)
    {
        QuestDict.TryGetValue(key, out QuestData data);
        return data;
    }
}
