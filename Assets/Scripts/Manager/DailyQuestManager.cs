using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DailyQuestManager : MonoBehaviour
{
    private GameManager gameManager;
    private List<DailyQuestData> activeQuests = new();

    public void Init(GameManager gm)
    {
        gameManager = gm;

    }

    public void LoadQuest()
    {

    }

    public void ProgressQuest(string questId, int amount)
    {
        var quest = activeQuests.Find(q => q.questId == questId);
        if (quest == null || quest.isCompleted)
        {
            return;
        }

        quest.currentAmount += amount;

        if (quest.currentAmount >= quest.goalAmount)
        {
            quest.isCompleted = true;
            Debug.Log("퀘스트 완료 ");
        }


    }

    public void ClaimReward(string questId)
    {
        var quest = activeQuests.Find(q => q.questId == questId);
        if (quest == null || !quest.isCompleted || quest.isClaimed)
        {
            return;
        }
        quest.isClaimed = true;

        gameManager.ForgeManager.AddDia(quest.rewardCount);
    }

    public List<DailyQuestData> GetActiveQuests() => activeQuests;


}