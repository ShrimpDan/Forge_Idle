using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonalityData
{
    /// <summary>
    /// 키값
    /// </summary>
    public string Key;

    /// <summary>
    /// 성격의 이름
    /// </summary>
    public string personalityName;

    /// <summary>
    /// 제자의 이름
    /// </summary>
    public string Name;

    /// <summary>
    /// 성격의 티어
    /// </summary>
    public int tier;

    /// <summary>
    /// 제작 능력치 배율
    /// </summary>
    public float craftingMultiplier;

    /// <summary>
    /// 채광 능력치 배율
    /// </summary>
    public float miningMultiplier;

    /// <summary>
    /// 판매 능력치 배율
    /// </summary>
    public float sellingMultiplier;
}

public class PersonalityDataLoader
{
    public List<PersonalityData> DataList { get; private set; }
    public Dictionary<string, PersonalityData> DataDict { get; private set; }

    public PersonalityDataLoader(string path = "Data/personality_data")
    {
        string jsonData = Resources.Load<TextAsset>(path).text;
        DataList = JsonUtility.FromJson<Wrapper>(jsonData).Items;

        DataDict = new Dictionary<string, PersonalityData>();
        foreach (var item in DataList)
        {
            DataDict.Add(item.Key, item);
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<PersonalityData> Items;
    }

    public PersonalityData GetByKey(string key)
    {
        DataDict.TryGetValue(key, out PersonalityData data);
        return data;
    }
}

