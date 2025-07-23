using System.Collections;
using System.Collections.Generic;
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


    public void InitButton(DailyQuestData questData , DailyQuestManager dailyQuestManager)
    {
        questNameText.text = questData.title;
        questInfo.text = questData.questInfo;

       // UpdateProgress(questData.currentAmount, questData.goalAmount);
        //UpdateReward(questData.rewardCount);

        claimButton.SetActive(!questData.isCompleted || !questData.isClaimed);
        claimButton.GetComponent<Button>().onClick.AddListener(() => 
        {
            dailyQuestManager.ClaimReward(questData.questId); // 퀘스트 보상 받기
            
            claimButton.SetActive(false);
        });
    }
    


    
}
