using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerType
{
    Normal,
    Regular,
    Nuisance
}

public enum CustomerJob
{
    Woodcutter,
    Farmer,
    Miner,
    Warrior,
    Archer,
    Tanker,
    Assassin
}

[System.Serializable]
public class CustomerData
{
    /// <summary>
    /// 키 값
    /// </summary>
    public string Key;

    /// <summary>
    /// 손님의 직업
    /// </summary>
    public CustomerJob job;

    /// <summary>
    /// 손님의 타입
    /// </summary>
    public CustomerType type;

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed;

    // ==== [추가] 해당 손님이 선호/착용 가능한 무기(RecipeKey) 리스트 ====
    public List<string> WeaponKeys;
}

public class CustomerDataLoader
{
    public List<CustomerData> ItemsList { get; private set; }
    public Dictionary<string, CustomerData> ItemsDict { get; private set; }

    public CustomerDataLoader(string path = "Data/customer_data")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<string, CustomerData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.Key, item);
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<CustomerData> Items;
    }

    public CustomerData GetByKey(string key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
}
