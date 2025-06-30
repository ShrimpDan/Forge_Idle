using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingData
{
    public string ItemKey;
    public float craftTime;
    public float craftCost;
    public List<RequiredResources> RequiredResources;
    public float sellCost;
}

[System.Serializable]
public class RequiredResources
{
    public string ResourceKey;
    public int Amount;
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
            Debug.LogWarning("Crafting 데이터가 존재하지 않습니다.");
            return;
        }

        List<CraftingDataFlat> flatList = JsonUtility.FromJson<Wrapper<CraftingDataFlat>>(json.text).Items;

        CraftingList = new List<CraftingData>();
        CraftingDict = new Dictionary<string, CraftingData>();

        foreach (var flat in flatList)
        {
            CraftingData data = new CraftingData
            {
                ItemKey = flat.Key,
                craftTime = flat.craftTime,
                craftCost = flat.craftCost,
                sellCost = flat.sellCost,
                RequiredResources = new List<RequiredResources>()
            };

            for (int i = 0; i < flat.resourceKeys.Count; i++)
            {
                RequiredResources resources = new RequiredResources
                {
                    ResourceKey = flat.resourceKeys[i],
                    Amount = flat.resourceAmount[i]
                };

                data.RequiredResources.Add(resources);
            }

            CraftingList.Add(data);
            CraftingDict[data.ItemKey] = data;
        }

        foreach (var data in CraftingList)
            {
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
        CraftingDict.TryGetValue(key, out CraftingData data);
        return data;
    }
}

[System.Serializable]
public class CraftingDataFlat
{
    public string Key;
    public float craftTime;
    public float craftCost;
    public float sellCost;
    public List<string> resourceKeys;
    public List<int> resourceAmount;
}