using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class WeaponSellingSaveData
{
    public List<string> CraftingKeys;
}

public class WeaponSellingSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "weaponselling_save.json");

    public static void Save(WeaponSellingSystem system)
    {
        var saveData = system.SaveToData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[저장 시스템] 무기 판매정보 저장이 완료되었습니다.\n 경로: {SavePath}");
    }

    public static void Load(WeaponSellingSystem system)
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[저장 시스템] 무기 판매정보 파일이 존재하지않습니다.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<WeaponSellingSaveData>(json);
        system.LoadFromSaveData(saveData);
        Debug.Log("[저장 시스템] 무기 판매정보 로드 완료.");
    }

    public static void Delete(WeaponSellingSystem system)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            system.InitDictionary();
            Debug.Log("WeaponSellSaveData 삭제");
        }
    }
}

public class WeaponSellingSaveHandler : ISaveHandler
{
    private WeaponSellingSystem sellingSystem;

    public WeaponSellingSaveHandler(WeaponSellingSystem system)
    {
        sellingSystem = system;
    }

    public void Load()
    {
        WeaponSellingSaveSystem.Load(sellingSystem);
    }

    public void Save()
    {
        WeaponSellingSaveSystem.Save(sellingSystem);
    }

    public void Delete()
    {
        WeaponSellingSaveSystem.Delete(sellingSystem);
    }
}