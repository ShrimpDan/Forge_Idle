using System.Collections.Generic;

public class DungeonSystem
{
    private GameManager gameManager;
    private DungeonDataLoader dungeonLoader;

    public HashSet<string> UnlockDungeonKeys { get; private set; }
    public DungeonData CurrentDungeon { get; private set; }

    public DungeonSystem(GameManager gameManager)
    {
        this.gameManager = gameManager;
        dungeonLoader = gameManager.DataManager.DungeonDataLoader;
    }

    public void UnlockNextDungeon(DungeonData data)
    {
        string nextKey = dungeonLoader.GetNextDungeonKey(data);

        if (nextKey != null)
        {
            UnlockDungeonKeys.Add(dungeonLoader.GetNextDungeonKey(data));
            gameManager.UIManager.ReLoadUI(UIName.DungeonWindow);
        }
    }

    public bool CheckUnlock(string key)
    {
        if (UnlockDungeonKeys.Contains(key))
        {
            return true;
        }

        return false;
    }

    public void EnterDungeon(DungeonData data)
    {
        CurrentDungeon = data;
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.Dungeon, true);
    }

    public void ExitDungeon()
    {
        CurrentDungeon = null;
    }

    public DungeonSaveData ToSaveData()
    {
        DungeonSaveData saveData = new DungeonSaveData();
        saveData.dungeonKeys = new List<string>();

        foreach (var key in UnlockDungeonKeys)
        {
            saveData.dungeonKeys.Add(key);
        }

        return saveData;
    }

    public void LoadFromSaveData(DungeonSaveData saveData)
    {
        UnlockDungeonKeys = new HashSet<string>();

        if (saveData == null)
        {
            UnlockDungeonKeys.Add(dungeonLoader.DungeonLists[0].Key);
            return;
        }

        foreach (var key in saveData.dungeonKeys)
        {
            UnlockDungeonKeys.Add(key);
        }
    }

    public void ClearUnlockDungeon()
    {
        UnlockDungeonKeys.Clear();
        UnlockDungeonKeys.Add(dungeonLoader.DungeonLists[0].Key);
    }
}
