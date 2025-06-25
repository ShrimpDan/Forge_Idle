using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public float Attack;
    public float AttackInterval;
    public int EnhanceMax;
}

[System.Serializable]
public class UpgradeInfo
{
    public int CurrentEnhanceLevel;
    public int MaxEnhanceLevel;

    public float AttackBonusPerLevel;
    public float IntervalReductionPerLevel;

    public float GetEnhancedAttack(float baseAttack)
    {
        return baseAttack + AttackBonusPerLevel * CurrentEnhanceLevel;
    }

    public float GetEnhancedInterval(float baseInterval)
    {
        return Mathf.Max(0.1f, baseInterval - IntervalReductionPerLevel * CurrentEnhanceLevel);
    }
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
    public string ItemKey;
    public ItemType ItemType;
    public string Name;
    public string Description;
    public string IconPath;

    public WeaponStats WeaponStats;
    public UpgradeInfo UpgradeInfo;
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
            Debug.LogWarning("Data가 존재하지않습니다.");
            return;
        }

        ItemList = JsonUtility.FromJson<Wrapper>(json.text).Items;
        ItemDict = new Dictionary<string, ItemData>();

        foreach (var item in ItemList)
        {
            ItemDict[item.ItemKey] = item;
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


