using System.IO;
using UnityEngine;

[System.Serializable]
public class ForgeData
{
    // 제작 관련 스탯
    public float CraftSpeedMultiplier;
    public float RareItemChance;

    // 광산 관련 스탯
    public float MiningYieldPerMinute;
    public float MaxMiningCapacity;

    // 판매 관련 스탯
    public float SellPriceMultiplier;
    public float CustomerSpawnRate;

    // 레벨 및 명성치
    public int Level;
    public int CurrentFame;
    public int MaxFame;
    public int TotalFame;

    // 재화
    public int Gold;
    public int Dia;
}

public static class ForgeSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "forge_data.json");

    public static void SaveForge(Forge forge)
    {
        var data = forge.SaveData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[저장 시스템] ForgeData 저장이 완료되었습니다.\n 경로: {SavePath}");
    }

    public static void LoadForge(Forge forge)
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[저장 시스템] ForgeData가 존재하지 않습니다.");

            var newData = GetDefaultData();
            forge.LoadData(newData);
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<ForgeData>(json.ToString());
        forge.LoadData(data);
    }

    public static void Delete(Forge forge)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            forge.ClearForge();
            var newData = GetDefaultData();
            forge.LoadData(newData);

            Debug.Log("ForgeData 삭제");
        }
    }

    private static ForgeData GetDefaultData()
    {
        return new ForgeData
        {
            // 제작 기본 스탯
            CraftSpeedMultiplier = 1f,
            RareItemChance = 0.1f,

            // 광산 기본 스탯
            MiningYieldPerMinute = 0.0f,
            MaxMiningCapacity = 0.0f,

            // 판매 기본 스탯
            SellPriceMultiplier = 1.0f,
            CustomerSpawnRate = 30.0f,

            // 레벨 및 명성치 기본값
            Level = 1,
            CurrentFame = 0,
            MaxFame = 100,
            TotalFame = 0,

            // 재화 기본값
            Gold = 0,
            Dia = 0
        };
    }
}

public class ForgeSaveHandeler : ISaveHandler
{
    private Forge forge;

    public ForgeSaveHandeler(Forge forge)
    {
        this.forge = forge;
    }

    public void Save()
    {
        ForgeSaveSystem.SaveForge(forge);
    }

    public void Load()
    {
        ForgeSaveSystem.LoadForge(forge);
    }

    public void Delete()
    {
        ForgeSaveSystem.Delete(forge);
    }
}
