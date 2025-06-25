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

    public CraftingDataLoader(string path = "Items/crafting_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("Crafting 데이터가 존재하지 않습니다.");
            return;
        }

        CraftingList = JsonUtility.FromJson<Wrapper>(json.text).Craftings;
        CraftingDict = new Dictionary<string, CraftingData>();

        foreach (var data in CraftingList)
        {
            CraftingDict[data.ItemKey] = data;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<CraftingData> Craftings;
    }

    public CraftingData GetDataByKey(string key)
    {
        CraftingDict.TryGetValue(key, out CraftingData data);
        return data;
    }
}