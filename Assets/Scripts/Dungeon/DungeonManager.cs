using System.Collections;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public GameManager GameManager { get; private set; }

    public DungeonData DungeonData { get; private set; }

    public WeaponHandler WeaponHandler { get; private set; }
    public MonsterHandler MonsterHandler { get; private set; }
    public RewardHandler RewardHandler { get; private set; }
    public DungeonUI DungeonUI { get; private set; }

    private float time = 0f;
    public bool IsRunning { get; private set; } = true;

    void Start()
    {
        GameManager = GameManager.Instance;
        Init();
    }

    public void Init()
    {
        DungeonData = GameManager.CurrentDungeon;

        DungeonUI = FindObjectOfType<DungeonUI>();
        DungeonUI.Init(this);

        WeaponHandler = GetComponent<WeaponHandler>();
        MonsterHandler = GetComponent<MonsterHandler>();
        RewardHandler = GetComponent<RewardHandler>();

        WeaponHandler.Init(this, GameManager.Inventory.GetEquippedWeapons());
        MonsterHandler.Init(this);
        RewardHandler.Init(GameManager.Inventory, DungeonUI);

        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        time = DungeonData.Duration;

        while (IsRunning)
        {
            time -= 0.1f;
            DungeonUI.UpdateTimerUI(time, DungeonData.Duration);

            if (time <= 0)
            {
                IsRunning = false;

                // 시간이 다 되면 클리어 실패
                DungeonClear(false);
            }

            yield return WaitForSecondsCache.Wait(0.1f);
        }
    }

    public void AddReward(int amount)
    {
        string ranRewardKey = DungeonData.RewardItemKeys[Random.Range(0, DungeonData.RewardItemKeys.Count)];
        ItemData rewardItem = GameManager.DataManager.ItemLoader.GetItemByKey(ranRewardKey);
        RewardHandler.AddReward(rewardItem, amount);
    }

    public void DungeonClear(bool isClear)
    {
        IsRunning = false;
        DungeonUI.OpenClearPopup(isClear);
    }

    public void ExitDungeon()
    {
        RewardHandler.ApplyReward();
        GameManager.ExitDungeon();
        LoadSceneManager.Instance.UnLoadScene(SceneType.Dungeon);
    }

    public void BackToMain()
    {
        GameManager.ExitDungeon();
        LoadSceneManager.Instance.UnLoadScene(SceneType.Dungeon);
    }
}
