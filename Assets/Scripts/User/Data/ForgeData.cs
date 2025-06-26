using System.IO;
using UnityEngine;

[System.Serializable]
public class ForgeData
{
    public float CraftTimeMultiplier;
    public float SellPriceMultiplier;
    public float RareItemChance;
    public float CustomerSpawnDelay ;

    public int Level;
    public int CurrentFame;
    public int MaxFame;
    public int TotalFame;

    public int Gold;
    public int Dia;
}

public static class ForgeDataSaveLoader
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "forge_data.json");

    public static void Save(ForgeData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static ForgeData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("ForgeData 존재하지 않습니다.");

            ForgeData newData = GetDefaultData();
            Save(newData);

            return newData;
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
            CustomerSpawnDelay  = 5f,
            Level = 1,
            MaxFame = 100,
            CurrentFame = 0,
            TotalFame = 0,
            Gold = 5000,
            Dia = 0
        };
    }
}
