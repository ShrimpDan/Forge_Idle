using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssistantData
{
    /// <summary>
    /// 키 값
    /// </summary>
    public string Key;

    /// <summary>
    /// 제자이름
    /// </summary>
    public string Name;

    /// <summary>
    /// 제자 특화 키값
    /// </summary>
    public string specializationKey;

    /// <summary>
    /// 제자 성격 키값
    /// </summary>
    public string personalityKey;

    /// <summary>
    /// 제자 설명
    /// </summary>
    public string customerInfo;

    /// <summary>
    /// 제자아이콘경로
    /// </summary>
    public string iconPath;

    /// <summary>
    /// 등급
    /// </summary>
    public string grade;

}
public class AssistantDataLoader
{
    public List<AssistantData> ItemsList { get; private set; }
    public Dictionary<string, AssistantData> ItemsDict { get; private set; }

    public List<AssistantData> Tier1List { get; private set; }
    public List<AssistantData> Tier2List { get; private set; }
    public List<AssistantData> Tier3List { get; private set; }
    public List<AssistantData> Tier4List { get; private set; }
    public List<AssistantData> Tier5List { get; private set; }

    public AssistantDataLoader(string path = "Data/assistant_data")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<string, AssistantData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
        }

        InitListByTier();
    }

    [Serializable]
    private class Wrapper
    {
        public List<AssistantData> Items;
    }

    public AssistantData GetByKey(string key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }

    private void InitListByTier()
    {
        Tier1List = ItemsList.FindAll(a => a.grade == "UR");
        Tier2List = ItemsList.FindAll(a => a.grade == "SSR");
        Tier3List = ItemsList.FindAll(a => a.grade == "SR");
        Tier4List = ItemsList.FindAll(a => a.grade == "R");
        Tier5List = ItemsList.FindAll(a => a.grade == "N");
    }
}
