using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ForgeTypeData
{
    public ForgeType Type;
    public List<ForgeUpgradeSaveData> UpgradeLevels = new();
    public List<string> EquippedAssistantKeys = new();
}

[System.Serializable]
public class ForgeUpgradeSaveData
{
    public ForgeUpgradeType UpgradeType;
    public int Level;
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
        Debug.Log($"[저장 시스템] {forge} 저장이 완료되었습니다.\n 경로: {path}");
    }

    public void LoadForge(Forge forge)
    {
        string path = GetSavePath(forge.ForgeType);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[저장 시스템] {forge}의 데이터가 존재하지 않습니다.");

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

            Debug.Log($"{forge} 데이터 삭제");
        }
    }

    private ForgeTypeData GetDefaultData(ForgeType type)
    {
        List<ForgeUpgradeType> upgradeTypes = new List<ForgeUpgradeType>
        {
            ForgeUpgradeType.IncreaseSellPrice,
            ForgeUpgradeType.IncreaseHighGradeRecipeChance,
            ForgeUpgradeType.ReduceCustomerSpawnDelay,
            ForgeUpgradeType.ReduceAutoCraftingTime,
            ForgeUpgradeType.IncreasePerfectCraftChance
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
            EquippedAssistantKeys = null
        };
    }

}
