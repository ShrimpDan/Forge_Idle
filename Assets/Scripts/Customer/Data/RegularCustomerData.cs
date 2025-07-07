using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CustomerRarity
{
    Common,
    Rare,
    Epic,
    Unique,
    Legendary
}

[System.Serializable]
public class RegularCustomerData //담기는 데이터
{
    /// <summary>
    /// 키 값
    /// </summary>
    public string Key;

    /// <summary>
    /// 손님 유형 키값
    /// </summary>
    public string customerKey;

    /// <summary>
    /// 손님이름
    /// </summary>
    public string customerName;

    /// <summary>
    /// 손님정보
    /// </summary>
    public string customerInfo;

    /// <summary>
    /// 손님아이콘경로
    /// </summary>
    public string iconPath;

    /// <summary>
    /// 희귀도
    /// </summary>
    public CustomerRarity rarity;
}

public class RegularDataLoader //데이터 가져와줌
{
    public List<RegularCustomerData> ItemsList { get; private set; }
    public Dictionary<string, RegularCustomerData> ItemsDict { get; private set; }

    public RegularDataLoader(string path = "Data/regular_data")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<string, RegularCustomerData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<RegularCustomerData> Items;
    }

    public RegularCustomerData GetByKey(string key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
}
