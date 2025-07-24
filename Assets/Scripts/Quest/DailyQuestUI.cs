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


        Button btn = claimButton.GetComponent<Button>();

        btn.interactable = questData.isCompleted && !questData.isClaimed;

       // btn.onClick.RemoveAllListeners();


        if (!questData.isClaimed && questData.currentAmount == 0)
        {
            buttonText.text = "수락하기";
            btn.interactable = true;

            btn.onClick.AddListener(() =>
            {
                questData.isClaimed = true;
                buttonText.text = "진행중";
                btn.interactable = false;
            });
        }

        else if (questData.isClaimed && !questData.isCompleted)
        {
            buttonText.text = "진행중";
            btn.interactable = false;
        }
      
        else if (questData.isCompleted && !questData.isClaimed)
        {
            buttonText.text = "보상 받기";
            btn.interactable = true;

            btn.onClick.AddListener(() =>
            {
                dailyQuestManager.ClaimReward(questData.questId);
                btn.interactable = false;
                buttonText.text = "수락함";
            });
        }
      
        else if (questData.isClaimed)
        {
            buttonText.text = "수락함";
            btn.interactable = false;
        }
    }
    


    
}
