using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ForgeTypeData
{
    public ForgeType Type;
    public List<ForgeUpgradeSaveData> UpgradeLevels = new();
    public List<ForgeRecipeSaveData> Recipes = new();
}

[System.Serializable]
public class ForgeUpgradeSaveData
{
    public ForgeUpgradeType UpgradeType;
    public int Level;
}

[System.Serializable]
public class ForgeRecipeSaveData
{
    public WeaponType WeaponType;
    public List<string> RecipeKeys;
}

public class ForgeTypeSaveSystem
{
    private string GetSavePath(ForgeType forgeType)
    {
        return Path.Combine(Application.persistentDataPath, $"forge_{forgeType.ToString().ToLower()}.json");
    }

    public void SaveForgeType(Forge forge)
    {
        var data = forge.SaveToData();
        string path = GetSavePath(forge.ForgeType);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public void LoadForge(Forge forge)
    {
        string path = GetSavePath(forge.ForgeType);

        if (!File.Exists(path))
        {
            var newData = GetDefaultData(forge.ForgeType);
            forge.LoadFromData(newData);
            return;
        }

        string json = File.ReadAllText(GetSavePath(forge.ForgeType));
        var data = JsonUtility.FromJson<ForgeTypeData>(json.ToString());
        forge.LoadFromData(data);
    }

    public void Delete(Forge forge)
    {
        string path = GetSavePath(forge.ForgeType);

        if (File.Exists(path))
        {
            File.Delete(path);

            var newData = GetDefaultData(forge.ForgeType);
            forge.LoadFromData(newData);
        }
    }

    private ForgeTypeData GetDefaultData(ForgeType type)
    {
        List<ForgeUpgradeType> upgradeTypes = new List<ForgeUpgradeType>
        {
            ForgeUpgradeType.IncreaseSellPrice,
            ForgeUpgradeType.IncreaseExpensiveRecipeChance,
            ForgeUpgradeType.ReduceCustomerSpawnDelay,
            ForgeUpgradeType.ReduceAutoCraftingTime,
            ForgeUpgradeType.IncreasePerfectCraftChance,
            ForgeUpgradeType.UpgradeInterior
        };

        List<ForgeUpgradeSaveData> upgradeLevels = new List<ForgeUpgradeSaveData>();

        for (int i = 0; i < upgradeTypes.Count; i++)
        {
            ForgeUpgradeSaveData data = new ForgeUpgradeSaveData
            {
                UpgradeType = upgradeTypes[i],
                Level = 1
            };

            upgradeLevels.Add(data);
        }
        return new ForgeTypeData
        {
            Type = type,
            UpgradeLevels = upgradeLevels,
        };
    }

}
