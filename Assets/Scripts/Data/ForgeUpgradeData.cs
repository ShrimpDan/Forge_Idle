using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ForgeUpgradeData
{
    public ForgeType ForgeType;
    public ForgeUpgradeType UpgradeType;
    public int Level;
    public float Value;
    public int Cost;
}

public class ForgeUpgradeDataLoader
{
    public Dictionary<(ForgeType, ForgeUpgradeType), Dictionary<int, ForgeUpgradeData>> dataTable;

    public ForgeUpgradeDataLoader(string path = "Data/forge_upgrade_data")
    {
        dataTable = new();

        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("ForgeUpgradeData가 존재하지않습니다.");
            return;
        }

        var dataList = JsonUtility.FromJson<Wrapper>(json.text).Items;

        foreach (var data in dataList)
        {
            var key = (data.ForgeType, data.UpgradeType);

            if (!dataTable.ContainsKey(key))
                dataTable[key] = new Dictionary<int, ForgeUpgradeData>();

            dataTable[key][data.Level] = data;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<ForgeUpgradeData> Items;
    }

    public float GetValue(ForgeType forgeType, ForgeUpgradeType upgradeType, int level)
    {
        var key = (forgeType, upgradeType);

        if (dataTable.TryGetValue(key, out var levelDict))
        {
            if (levelDict.TryGetValue(level, out var data))
                return data.Value;
        }

        return -1f;
    }

    public int GetCost(ForgeType forgeType, ForgeUpgradeType upgradeType, int level)
    {
        var key = (forgeType, upgradeType);

        if (dataTable.TryGetValue(key, out var levelDict))
        {
            if (levelDict.TryGetValue(level, out var data))
                return data.Cost;
        }

        return -1;
    }

    public int GetMaxLevel(ForgeType forgeType, ForgeUpgradeType upgradeType)
    {
        var key = (forgeType, upgradeType);
        if (dataTable.TryGetValue(key, out var levelDict))
        {
            int max = 0;
            foreach (var level in levelDict.Keys)
            {
                if (level > max)
                    max = level;
            }
            return max;
        }

        return 0;
    }
}