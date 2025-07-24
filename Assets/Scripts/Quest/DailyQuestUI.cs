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


    public void InitButton(DailyQuestData questData , DailyQuestManager dailyQuestManager)
    {
        questNameText.text = questData.title;
        questInfo.text = questData.questInfo;
        questProgressText.text = $"{questData.currentAmount / questData.goalAmount}";
        questRewardText.text = $"다이아 : {questData.rewardCount}";
        Button btn = claimButton.GetComponent<Button>();

        btn.interactable = questData.isCompleted && !questData.isClaimed;

        btn.onClick.RemoveAllListeners(); // Test


        if (!questData.isClaimed && questData.currentAmount == 0)
        {
            // 수락 전
            buttonText.text = "수락하기";
            btn.interactable = true;
            btn.onClick.AddListener(() =>
            {
                questData.isClaimed = true;
                buttonText.text = "진행중";
                btn.interactable = false;
            });
        }
        else if (questData.isCompleted && questData.isClaimed)
        {
           // 완료 + 수락 상태 → 보상 받기
            buttonText.text = "보상 받기";
            btn.interactable = true;
            btn.onClick.AddListener(() =>
            {
                dailyQuestManager.ClaimReward(questData.questId);
                btn.interactable = false;
                buttonText.text = "완료함";
            });
        }
        else if (questData.isClaimed && !questData.isCompleted)
        {
            // 진행 중
            buttonText.text = "진행중";
            btn.interactable = false;
        }
        else if (questData.isClaimed)
        {
            // 이미 보상 받은 상태
            buttonText.text = "수락함";
            btn.interactable = false;
        }
    
    }
    


    
}
