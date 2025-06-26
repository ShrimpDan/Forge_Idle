using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    private GameManager gameManager;

    private TestDungeonData dungeonData;

    private WeaponHandler weaponHandler;
    private MonsterHandler monsterHandler;
    private RewardHandler rewardHandler;
    private DungeonUI dungeonUI;

    [SerializeField] float dungeonDuration = 60f;
    private float time = 0f;
    private WaitForSeconds waitTime = new WaitForSeconds(1f);

    public bool IsRunning { get; private set; } = true;

    void Start()
    {
        gameManager = GameManager.Instance;
        Init();

        StartCoroutine(TimerCoroutine());
    }

    public void Init()
    {
        dungeonData = new TestDungeonData
        {
            Key = "TestDungeon",
            DungeonName = "TestDungeon",
            MonsterHp = 30,
            BossHp = 100
        };

        dungeonUI = FindObjectOfType<DungeonUI>();
        dungeonUI.Init(this, gameManager.UIManager);

        weaponHandler = GetComponent<WeaponHandler>();
        monsterHandler = GetComponent<MonsterHandler>();
        rewardHandler = GetComponent<RewardHandler>();

        weaponHandler.Init(this, gameManager.Inventory.GetEquippedWeapons(), monsterHandler);
        monsterHandler.Init(this, dungeonUI, dungeonData);
        rewardHandler.Init();
    }

    private IEnumerator TimerCoroutine()
    {
        time = dungeonDuration;

        while (IsRunning)
        {
            time -= 1;
            dungeonUI.UpdateTimerUI(time, dungeonDuration);

            if (time <= 0)
            {
                IsRunning = false;

                // 시간이 다 되면 클리어 실패
                DungeonClear(false);
            }

            yield return waitTime;
        }
    }

    public void DungeonClear(bool isClear)
    {
        IsRunning = false;
        dungeonUI.OpenClearPopup(isClear);
    }

    public void ExitDungeon()
    {
        SceneManager.UnloadSceneAsync("DungeonScene");
    }
}
