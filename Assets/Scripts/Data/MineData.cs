using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MineData
{
    public string Key;
    public string MineName;
    public int CollectMin;
    public int CollectMax;
    public float CollectRatePerHour;
    public List<string> RewardMineralKeys;
}

public class MineLoader
{
    public List<MineData> DataList { get; private set; }
    public Dictionary<string, MineData> DataDict { get; private set; }

    public MineLoader(string path = "Data/mine_data")
    {
        string jsonData = Resources.Load<TextAsset>(path).text;
        DataList = JsonUtility.FromJson<Wrapper>(jsonData).Mines; // <-- ÀÌ ºÎºÐ!!
        DataDict = new Dictionary<string, MineData>();
        foreach (var item in DataList)
            DataDict.Add(item.Key, item);
    }

    [System.Serializable]
    private class Wrapper { public List<MineData> Mines; }

    public MineData GetByKey(string key) => DataDict.TryGetValue(key, out var data) ? data : null;
}

