using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecializationData
{
    public string Key;
    public int tier;
    public SpecializationType specializationType;
    public List<string> statNames;
    public List<float> statValues;
}

public class SpecializationDataLoader
{
    public List<SpecializationData> DataList { get; private set; }
    public Dictionary<string, SpecializationData> DataDict { get; private set; }

    public SpecializationDataLoader(string path = "data/specialization_data")
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

    public SpecializationData GetByTierAndType(int tier, SpecializationType type)
    {
        foreach (var data in DataList)
        {
            if (data.tier == tier && data.specializationType == type)
            {
                return data;
            }
        }
        return null;
    }
}

