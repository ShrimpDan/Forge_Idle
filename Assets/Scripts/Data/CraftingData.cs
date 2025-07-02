using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequiredResources
{
    public string ResourceKey;
    public int Amount;
}

[System.Serializable]
public class CraftingData
{
    public string ItemKey;
    public float craftTime;
    public float craftCost;
    public float sellCost;
    public List<RequiredResources> RequiredResources;
}

[System.Serializable]
public class CraftingDataFlat
{
    public string Key;
    public float craftTime;
    public float craftCost;
    public float sellCost;
    public List<string> resourceKey;
    public List<int> amount;
}

public class CraftingDataLoader
{
    public List<CraftingData> CraftingList { get; private set; }
    public Dictionary<string, CraftingData> CraftingDict { get; private set; }

    public CraftingDataLoader(string path = "Data/crafting_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);
        if (json == null)
        {
            Debug.LogWarning("Crafting data not found.");
            return;
        }

        var flatWrapper = JsonUtility.FromJson<Wrapper<CraftingDataFlat>>(json.text);
        if (flatWrapper == null || flatWrapper.Items == null)
        {
            Debug.LogWarning("Crafting data parsing failed.");
            return;
        }

        CraftingList = new List<CraftingData>();
        CraftingDict = new Dictionary<string, CraftingData>();

        foreach (var flat in flatWrapper.Items)
        {
            var data = new CraftingData
            {
                ItemKey = flat.Key,
                craftTime = flat.craftTime,
                craftCost = flat.craftCost,
                sellCost = flat.sellCost,
                RequiredResources = new List<RequiredResources>()
            };

            if (flat.resourceKey != null && flat.amount != null)
            {
                int count = Mathf.Min(flat.resourceKey.Count, flat.amount.Count);
                for (int i = 0; i < count; i++)
                {
                    data.RequiredResources.Add(new RequiredResources
                    {
                        ResourceKey = flat.resourceKey[i],
                        Amount = flat.amount[i]
                    });
                }
            }

            CraftingList.Add(data);
            CraftingDict[data.ItemKey] = data;
        }
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public CraftingData GetDataByKey(string key)
    {
        CraftingDict.TryGetValue(key, out var data);
        return data;
    }
}
