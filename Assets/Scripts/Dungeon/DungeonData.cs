using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public int Key;
    public string DungeonName;
    public float MonsterHp;
    public float BossHp;
    public float Duration;
    public int MaxMonster;
    public List<string> RewardItemKeys;
}

public class DungeonDataLoader
{
    public List<DungeonData> DungeonLists { get; private set; }
    public Dictionary<int, DungeonData> DungeonDict { get; private set; }

    public DungeonDataLoader(string path = "Data/dungeon_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("던전 데이터가 존재하지않습니다.");
            return;
        }

        DungeonLists = JsonUtility.FromJson<Wrapper>(json.text).DataList;

        DungeonDict = new Dictionary<int, DungeonData>();
        foreach (var dungeon in DungeonLists)
        {
            DungeonDict[dungeon.Key] = dungeon;
        }
    }

    private class Wrapper
    {
        public List<DungeonData> DataList;
    }

    public DungeonData GetDungeonByKey(int key)
    {
        DungeonDict.TryGetValue(key, out DungeonData data);
        return data;
    }
}