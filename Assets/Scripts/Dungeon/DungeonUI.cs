using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private UIManager uIManager;

    [Header("Battle Info UI")]
    [SerializeField] private Image timeFill;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI monsterText;
    [SerializeField] private DungeonPopup dungeonPopup;

    [Header("Reward Info")]
    [SerializeField] GameObject rewardSlotPrefab;
    [SerializeField] Transform rewardRoot;
    private List<RewardSlot> rewardSlots;

    [Header("Exit UI")]
    [SerializeField] GameObject exitPopup;
    [SerializeField] Button exitBtn;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button cancleBtn;
    [SerializeField] Image blockRay;

    public void Init(DungeonManager dungeonManager)
    {
        this.dungeonManager = dungeonManager;
        rewardSlots = new List<RewardSlot>();

        exitBtn.onClick.AddListener(() =>
        {
            exitPopup.SetActive(true);
            blockRay.enabled = true;
        });
        cancleBtn.onClick.AddListener(() =>
        {
            exitPopup.SetActive(false);
            blockRay.enabled = false;
        });
        confirmBtn.onClick.AddListener(() => dungeonManager.BackToMain());
    }

    public void UpdateTimerUI(float current, float max)
    {
        timeFill.fillAmount = current / max;
        timeText.text = current.ToString("F1");
    }

    public void UpdateMonsterUI(int killedMonster, int maxMonster)
    {
        if (killedMonster < maxMonster)
            monsterText.text = $"{killedMonster}/{maxMonster}";
        else
            monsterText.text = "Boss";
    }

    public void OpenClearPopup(bool isClear)
    {
        if (isClear)
        {
            var ui = uIManager.OpenUI<RewardPopup>(UIName.RewardPopup);
            ui.Show(dungeonManager.RewardHandler.RewardItems);
            return;
        }

        dungeonPopup.Init(dungeonManager, isClear);
    }

    public void UpdateRewardInfo(ItemData item, int amount)
    {
        var existSlot = rewardSlots.Find(i => i.SlotItem.ItemKey == item.ItemKey);

        if (existSlot == null)
        {
            GameObject obj = Instantiate(rewardSlotPrefab, rewardRoot);

            if (obj.TryGetComponent(out RewardSlot slot))
            {
                slot.Init(item, amount);
                rewardSlots.Add(slot);
            }
        }
        else
        {
            existSlot.AddItem(amount);
        }
    }
}
