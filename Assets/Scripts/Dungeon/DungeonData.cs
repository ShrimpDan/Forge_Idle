using System.Collections;
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
    public List<string> RewardItemKeys;
}

public class DungeonDataLoader
{
    public List<DungeonData> DungeonLists { get; private set; }
    public Dictionary<int, DungeonData> DungeonDict { get; private set; }

    public DungeonDataLoader(string path = "Data/dungeon_data.json")
    {
        TextAsset json = Resources.Load<TextAsset>(path);
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