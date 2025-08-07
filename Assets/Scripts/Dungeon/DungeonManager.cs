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
    public float DungeonTimeScale { get; private set; } = 1f;
    public float TimeMultiplier => 1f / DungeonTimeScale;

    void Start()
    {
        GameManager = GameManager.Instance;
        Init();
    }

    public void Init()
    {
        DungeonData = GameManager.DungeonSystem.CurrentDungeon;

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

        float timerInterval = 0.1f;

        while (IsRunning)
        {
            time -= timerInterval;
            DungeonUI.UpdateTimerUI(time, DungeonData.Duration);

            if (time <= 0)
            {
                time = 0f;

                // 시간이 다 되면 클리어 실패
                DungeonClear(false);
            }

            yield return WaitForSecondsCache.Wait(timerInterval * TimeMultiplier);
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
        if (!IsRunning) return;
        
        IsRunning = false;

        if (isClear)
        {
            SoundManager.Instance.Play("SFX_BattleStageClear01");
            GameManager.DungeonSystem.UnlockNextDungeon(DungeonData);
            GameManager.Instance.DailyQuestManager.ProgressQuest("DungonClear", 1);
        }
        else
            SoundManager.Instance.Play("SFX_BattleDungeonGiveUp02");

        DungeonUI.OpenClearPopup(isClear);
    }

    public void ExitDungeon()
    {
        RewardHandler.ApplyReward();
        GameManager.ForgeManager.AddFame(DungeonData.RewardFame);
        GameManager.DungeonSystem.ExitDungeon();
        LoadSceneManager.Instance.UnLoadScene(SceneType.Dungeon);
    }

    public void BackToMain()
    {
        SoundManager.Instance.Play("SFX_BattleDungeonGiveUp01");

        GameManager.DungeonSystem.ExitDungeon();
        LoadSceneManager.Instance.UnLoadScene(SceneType.Dungeon);
    }

    public void SetDungeonSpeed(int speed)
    {
        DungeonTimeScale = (float)speed;
    }
}
