using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public string Key;
    public string DungeonName;
    public float MonsterHp;
    public float BossHp;
    public float Duration;
    public int MaxMonster;
    public int MinCount;
    public int MaxCount;
    public string BossMonsterKey;
    public List<string> NormalMonsterKeys;
    public List<string> RewardItemKeys;
}

public class DungeonDataLoader
{
    public List<DungeonData> DungeonLists { get; private set; }
    public Dictionary<string, DungeonData> DungeonDict { get; private set; }

    public DungeonDataLoader(string path = "Data/dungeon_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("던전 데이터가 존재하지않습니다.");
            return;
        }

        DungeonLists = JsonUtility.FromJson<Wrapper>(json.text).Items;

        DungeonDict = new Dictionary<string, DungeonData>();
        foreach (var dungeon in DungeonLists)
        {
            DungeonDict[dungeon.Key] = dungeon;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<DungeonData> Items;
    }

    public DungeonData GetDungeonByKey(string key)
    {
        DungeonDict.TryGetValue(key, out DungeonData data);
        return data;
    }

    public string GetNextDungeonKey(DungeonData curDungeon)
    {
        int idx = DungeonLists.IndexOf(curDungeon);

        if (idx + 1 < DungeonLists.Count)
        {
            DungeonData nextDungeon = DungeonLists[idx + 1];
            return nextDungeon.Key;
        }

        return null;
    }
}