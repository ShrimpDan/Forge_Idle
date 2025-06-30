using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecializationData
{
    /// <summary>
    /// 키 값
    /// </summary>
    public string Key;

    /// <summary>
    /// 티어
    /// </summary>
    public int tier;

    /// <summary>
    /// 특화 타입
    /// </summary>
    public SpecializationType specializationType;

    /// <summary>
    /// 스탯 이름들
    /// </summary>
    public List<string> statNames;

    /// <summary>
    /// 각 이름에 대한 값
    /// </summary>
    public List<float> statValues;
}

public class SpecializationDataLoader
{
    public List<SpecializationData> DataList { get; private set; }
    public Dictionary<string, SpecializationData> DataDict { get; private set; }

    public SpecializationDataLoader(string path = "data/specializtion_data")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        DataList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        DataDict = new Dictionary<string, SpecializationData>();
        foreach (var item in DataList)
        {
            DataDict.Add(item.Key, item);
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<SpecializationData> Items;
    }

    public SpecializationData GetByKey(string key)
    {
        DataDict.TryGetValue(key, out SpecializationData data);
        return data;
    }
}