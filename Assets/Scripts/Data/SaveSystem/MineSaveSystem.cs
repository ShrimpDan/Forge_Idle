using System.Collections.Generic;
using System.IO;
using UnityEngine;

// --- 저장 핸들러 (외부에서 호출) ---
public class MineSaveHandler : ISaveHandler
{
    private MineSceneManager mineSceneManager;

    public MineSaveHandler(MineSceneManager manager)
    {
        mineSceneManager = manager;
    }

    public void Save() => MineSaveSystem.SaveMineSystem(mineSceneManager);
    public void Load() => MineSaveSystem.LoadMineSystem(mineSceneManager);
    public void Delete() => MineSaveSystem.Delete(mineSceneManager);
}

// --- 저장 데이터 구조 ---
[System.Serializable]
public class MineSlotSaveData
{
    public string AssistantKey;
    public string AssignedTime;

    public bool IsBuffActive;
    public float BuffRemain;
    public bool IsCooldown;
    public float CooldownRemain;
}

[System.Serializable]
public class MineGroupSaveData
{
    public string MineKey;
    public List<MineSlotSaveData> Slots = new List<MineSlotSaveData>();
    public string LastCollectTime;
}

[System.Serializable]
public class MineSaveData
{
    public List<MineGroupSaveData> Groups = new List<MineGroupSaveData>();
    public List<bool> UnlockedMines = new List<bool>(); // 해금 상태 추가
}

// --- 저장/로드/삭제 시스템 ---
public class MineSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "mine_save.json");

    public static void SaveMineSystem(MineSceneManager mineSceneManager)
    {
        var saveData = mineSceneManager.ToSaveData();
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void LoadMineSystem(MineSceneManager mineSceneManager)
    {
        if (!File.Exists(SavePath))
        {
            mineSceneManager.LoadFromSaveData(null);
            return;
        }
        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<MineSaveData>(json);
        mineSceneManager.LoadFromSaveData(saveData);
    }

    public static void Delete(MineSceneManager mineSceneManager)
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            mineSceneManager.ClearAllSlots();
            mineSceneManager.ResetUnlockedMines(); // 해금 상태도 초기화
        }
    }
}
