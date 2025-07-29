using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> ResourceItems;
    public List<ItemSaveData> WeaponItems;
    public List<ItemSaveData> GemItems;
    public List<int> EquippedWeaponIndices; // 각 인덱스에 해당하는 WeaponItems의 인덱스 저장
}


[System.Serializable]
public class ItemSaveData
{
    public string ItemKey;
    public int Quantity;
    public int CurrentEnhanceLevel;
    public bool IsEquipped;
    public List<string> GemSocketIDs;
}

public class InventorySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "inventory_save.json");

    public static void SaveInventory(InventoryManager inventoryManager)
    {
        var saveData = inventoryManager.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadInventory(InventoryManager inventoryManager)
    {
        if (!File.Exists(SavePath))
        {
            inventoryManager.ClearInventory();
            return;
        }

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<InventorySaveData>(json);
        inventoryManager.LoadFromSaveData(saveData);
    }

    public static void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}

public class InventorySaveHandler : ISaveHandler
{
    private InventoryManager inventoryManager;

    public InventorySaveHandler(InventoryManager manager)
    {
        inventoryManager = manager;
    }

    public void Save()
    {
        InventorySaveSystem.SaveInventory(inventoryManager);
    }

    public void Load()
    {
        InventorySaveSystem.LoadInventory(inventoryManager);
    }

    public void Delete()
    {
        InventorySaveSystem.Delete();
    }
}
