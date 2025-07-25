using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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


    public void InitButton(DailyQuestLoader loader, DailyQuestManager dailyQuestManager)
    {
        questNameText.text = loader.data.title;
        questInfo.text = loader.data.questInfo;
        questProgressText.text = $"{loader.currentAmount} / {loader.data.goalAmount}";
        questRewardText.text = $"다이아 : {loader.data.rewardCount}";

        Button btn = claimButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        if (!loader.isClaimed && loader.currentAmount == 0)
        {
            buttonText.text = "수락하기";
            btn.interactable = true;
            btn.onClick.AddListener(() =>
            {
                loader.isClaimed = true;
                buttonText.text = "진행중";
                btn.interactable = false;
            });
        }
        else if (loader.isCompleted && loader.isClaimed)
        {
            buttonText.text = "보상 받기";
            btn.interactable = true;
            btn.onClick.AddListener(() =>
            {
                dailyQuestManager.ClaimReward(loader.data.questId);
                btn.interactable = false;
                buttonText.text = "완료함";
            });
        }
        else if (loader.isClaimed && !loader.isCompleted)
        {
            buttonText.text = "진행중";
            btn.interactable = false;
        }
        else if (loader.isClaimed)
        {
            buttonText.text = "수락함";
            btn.interactable = false;
        }

    }
    


    
}
