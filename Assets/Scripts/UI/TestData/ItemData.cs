using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentStats
{
    public int Attack;
    public int Defense;
    public int EnhanceMax;
}

[System.Serializable]
public class GemStats
{
    public float EnhanceMultiplier;
    public string EffectDescription;
}

[System.Serializable]
public class ItemData
{
    public string Key;
    public ItemType ItemType;
    public string Name;
    public string Description;
    public string IconPath;

    public EquipmentStats EquipmentStats;
    public GemStats GemStats;
}

public class ItemDataLoader
{
    public List<ItemData> ItemList { get; private set; }
    public Dictionary<string, ItemData> ItemDict { get; private set; }

    public ItemDataLoader(string path = "Items/item_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.Log("Data가 존재하지않습니다.");
            return;
        }

        ItemList = JsonUtility.FromJson<Wrapper>(json.text).Items;
        ItemDict = new Dictionary<string, ItemData>();

        foreach (var item in ItemList)
        {
            ItemDict[item.Key] = item;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<ItemData> Items;
    }

    public ItemData GetItemByKey(string key)
    {
        ItemDict.TryGetValue(key, out ItemData data);
        return data;
    }

    public ItemData GetRandomItem()
    {
        return ItemList[Random.Range(0, ItemList.Count)];
    }
}


