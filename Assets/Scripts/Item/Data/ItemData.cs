using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public float Attack;
    public float AttackInterval;
}

[System.Serializable]
public class UpgradeInfo
{
    public int MaxEnhanceLevel;
    public float AttackMultiplier;
    public float IntervalReductionPerLevel;
}

[System.Serializable]
public class GemStats
{
    public float GemMultiplier;
    public string GemEffectDescription;
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

    public ItemDataLoader(string path = "Data/item_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("Data가 존재하지않습니다.");
            return;
        }

        List<ItemDataFlat> flatList = JsonUtility.FromJson<Wrapper<ItemDataFlat>>(json.text).Items;
        ItemList = new List<ItemData>();
        ItemDict = new Dictionary<string, ItemData>();

        foreach (var flat in flatList)
        {
            ItemData data = new ItemData
            {
                ItemKey = flat.Key,
                ItemType = flat.ItemType,
                Name = flat.Name,
                Description = flat.Description,
                IconPath = flat.IconPath
            };

            // Weapon
            if (flat.Attack > 0 || flat.AttackInterval > 0)
            {
                data.WeaponStats = new WeaponStats
                {
                    Attack = flat.Attack,
                    AttackInterval = flat.AttackInterval
                };

                data.UpgradeInfo = new UpgradeInfo
                {
                    MaxEnhanceLevel = flat.EnhanceMax,
                    AttackMultiplier = flat.AttackMultiplier,
                    IntervalReductionPerLevel = flat.IntervalReduction
                };
            }

            // Gem
            if (flat.GemMultiplier > 0)
            {
                data.GemStats = new GemStats
                {
                    GemMultiplier = flat.GemMultiplier,
                    GemEffectDescription = flat.GemEffectDescription
                };
            }

            ItemList.Add(data);
            ItemDict[data.ItemKey] = data;
        }
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
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

[System.Serializable]
public class ItemDataFlat
{
    public string Key;
    public ItemType ItemType;
    public string Name;
    public string Description;
    public string IconPath;

    // Weapon
    public float Attack;
    public float AttackInterval;

    // Upgrade
    public int EnhanceMax;
    public float AttackMultiplier;
    public float IntervalReduction;

    // Gem
    public float GemMultiplier;
    public string GemEffectDescription;
}



