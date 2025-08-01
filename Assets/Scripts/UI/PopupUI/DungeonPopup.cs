using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPopup : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private RewardHandler rewardHandler;

    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private Button confirmButton;

    [SerializeField] private TextMeshProUGUI rewardFameText;
    [SerializeField] private Transform rewardRoot;
    [SerializeField] private GameObject rewardSlotPrefab;

    [SerializeField] private Image blockRay;

    public void Init(DungeonManager dungeonManager, bool isClear)
    {
        this.dungeonManager = dungeonManager;
        rewardHandler = dungeonManager.RewardHandler;

        if (isClear)
        {
            clearText.text = "던전 클리어 !!";
            rewardFameText.text = $"명성치: +{dungeonManager.DungeonData.RewardFame}";
        }
        else
        {
            clearText.text = "클리어 실패...";
            rewardFameText.text = $"명성치: -";
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnClickButton);

        CreateRewardSlots();
        gameObject.SetActive(true);

        blockRay.enabled = true;
    }

    private void OnClickButton()
    {
        dungeonManager.ExitDungeon();
    }

    private void CreateRewardSlots()
    {
        foreach (ItemData item in rewardHandler.RewardItems.Keys)
        {
            GameObject obj = Instantiate(rewardSlotPrefab, rewardRoot);

            if (obj.TryGetComponent(out RewardSlot slot))
            {
                slot.Init(item, rewardHandler.RewardItems[item]);
            }
        }
    }
}
