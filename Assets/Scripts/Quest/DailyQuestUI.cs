using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questInfo;

    [SerializeField] private TextMeshProUGUI questProgressText; // 진행도
    [SerializeField] private TextMeshProUGUI questRewardText; // 보상

    [SerializeField] private GameObject claimButton; // 보상 받기 버튼
    [SerializeField] private TextMeshProUGUI buttonText;

    private DailyQuestLoader currentLoader;
    private DailyQuestManager dailyQuestManager;


    public void InitButton(DailyQuestLoader loader, DailyQuestManager dailyQuestManager)
    {

        currentLoader = loader;
        this.dailyQuestManager = dailyQuestManager;

        questNameText.text = loader.data.title;
        questInfo.text = loader.data.questInfo;
        questProgressText.text = $"{loader.currentAmount} / {loader.data.goalAmount}";
        questRewardText.text = $"{loader.data.rewardCount}";

        Button btn = claimButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        bool isCompletedNow = loader.currentAmount >= loader.data.goalAmount;
        bool isRewardClaimed = loader.isRewardClaimed;


        if (loader.isCompleted && !loader.isRewardClaimed)
        {
            // 보상 받기 버튼 활성화
            buttonText.text = "보상 받기";
            btn.interactable = true;
            btn.onClick.AddListener(() =>
            {
                dailyQuestManager.ClaimReward(loader.data.questId);
                btn.interactable = false;
                buttonText.text = "완료함";
            });
        }
        else if (!loader.isCompleted)
        {
            // 진행 중
            buttonText.text = "진행중";
            btn.interactable = false;
        }
        else if (loader.isRewardClaimed)
        {
            // 완료 상태
            buttonText.text = "완료함";
            btn.interactable = false;
        }
    }
    public bool IsSameQuest(DailyQuestLoader loader)
    {
        return currentLoader?.data.questId == loader.data.questId;
    }

    public void Refresh(DailyQuestLoader loader)
    {
        InitButton(loader, dailyQuestManager);
    }
}
    


    

