using System.Collections.Generic;
using UnityEngine;

public class PersonalityJson
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
    /// 성격의 티어
    /// </summary>
    public int tier;

    /// <summary>
    /// 제작 능력치 배율
    /// </summary>
    public float craftingMultiplier;

    /// <summary>
    /// 강화 능력치 배율
    /// </summary>
    public float enhancingMultiplier;

    /// <summary>
    /// 판매 능력치 배율
    /// </summary>
    public float sellingMultiplier;
}

public class PersonalityDataLoader
{
    public List<PersonalityJson> DataList { get; private set; }
    public Dictionary<string, PersonalityJson> DataDict { get; private set; }

    public PersonalityDataLoader(string path = "Data/personality_data")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        DataList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        DataDict = new Dictionary<string, PersonalityJson>();
        foreach (var item in DataList)
        {
            DataDict.Add(item.Key, item);
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<PersonalityJson> Items;
    }

    public PersonalityJson GetByKey(string key)
    {
        DataDict.TryGetValue(key, out PersonalityJson data);
        return data;
    }
}

