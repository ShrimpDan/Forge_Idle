using System.Collections.Generic;
using System.IO;
using UnityEngine;

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


[System.Serializable]
public class MineSlotSaveData
{
    public string AssistantKey;   // ��ϵ� ��ý���Ʈ�� Key 
    public string AssignedTime;   // ��� �ð�
}

[System.Serializable]
public class MineGroupSaveData
{
    public string MineKey; // group key
    public List<MineSlotSaveData> Slots = new List<MineSlotSaveData>();
    public string LastCollectTime; // ���������� ���ҽ��� ������ �ð�
}

[System.Serializable]
public class MineSaveData
{
    public List<MineGroupSaveData> Groups = new List<MineGroupSaveData>();
}




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
            mineSceneManager.ClearAllSlots(); // �ʿ��� �ʱ�ȭ �޼���
        }
    }
}
