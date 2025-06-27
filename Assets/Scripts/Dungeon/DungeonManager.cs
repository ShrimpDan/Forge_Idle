using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameManager GameManager { get; private set; }

    public DungeonData DungeonData { get; private set; }

    public WeaponHandler WeaponHandler { get; private set; }
    public MonsterHandler MonsterHandler { get; private set; }
    public RewardHandler RewardHandler { get; private set; }
    public DungeonUI DungeonUI { get; private set; }

    [SerializeField] float dungeonDuration = 60f;
    private float time = 0f;

    public bool IsRunning { get; private set; } = true;

    void Start()
    {
        GameManager = GameManager.Instance;
        Init();

        StartCoroutine(TimerCoroutine());
    }

    public void Init()
    {
        DungeonData = new DungeonData
        {
            Key = 0,
            DungeonName = "TestDungeon",
            MonsterHp = 30,
            BossHp = 100,
            RewardItemKeys = new List<string>
            {
                "resource_string",
                "resource_iron",
                "resource_fabric",
                "resource_wood"
            }
        };

        DungeonUI = FindObjectOfType<DungeonUI>();
        DungeonUI.Init(this);

        WeaponHandler = GetComponent<WeaponHandler>();
        MonsterHandler = GetComponent<MonsterHandler>();
        RewardHandler = GetComponent<RewardHandler>();

        WeaponHandler.Init(this, GameManager.Inventory.GetEquippedWeapons());
        MonsterHandler.Init(this);
        RewardHandler.Init(GameManager.Inventory, DungeonUI);
    }

    private IEnumerator TimerCoroutine()
    {
        time = dungeonDuration;

        while (IsRunning)
        {
            time -= 0.1f;
            DungeonUI.UpdateTimerUI(time, dungeonDuration);

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
        LoadSceneManager.Instance.UnLoadScene(SceneType.Dungeon);
    }

    public void BackToMain()
    {
        LoadSceneManager.Instance.UnLoadScene(SceneType.Dungeon);
    }
}
