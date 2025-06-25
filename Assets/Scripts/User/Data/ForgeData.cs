using System.IO;
using UnityEngine;

[System.Serializable]
public class ForgeData
{
    public float CraftTimeMultiplier;
    public float SellPriceMultiplier;
    public float RareItemChance;
    public float CustomerPerSecond;

    public int Level;
    public float CurrentFame;
    public float MaxFame;
    public float TotalFame;

    public int Gold;
    public int Dia;
}

public static class ForgeDataSaveLoader
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "forge_data.json");

    public static void Save(Forge forge)
    {
        ForgeData data = new ForgeData
        {
            CraftTimeMultiplier = forge.CraftTimeMultiplier,
            SellPriceMultiplier = forge.SellPriceMultiplier,
            RareItemChance = forge.RareItemChance,
            CustomerPerSecond = forge.CustomerPerSecond,
            Level = forge.Level,
            MaxFame = forge.MaxFame,
            CurrentFame = forge.CurrentFame,
            TotalFame = forge.TotalFame,
            Gold = forge.Gold,
            Dia = forge.Dia
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static ForgeData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("ForgeData가 존재하지 않습니다.");
            return GetDefaultData();
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<ForgeData>(json.ToString());
    }

    public static void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("ForgeData 삭제");
        }
    }

    private static ForgeData GetDefaultData()
    {
        return new ForgeData
        {
            CraftTimeMultiplier = 1f,
            SellPriceMultiplier = 1f,
            RareItemChance = 0.1f,
            CustomerPerSecond = 1f,
            Level = 1,
            MaxFame = 100f,
            CurrentFame = 0f,
            TotalFame = 0f,
            Gold = 0,
            Dia = 0
        };
    }
}
