using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DailyQuestManager : MonoBehaviour
{

    #region  모든퀘스트 관련정보
    private int maxQuestClearCount = 5;
    private bool isAllClear = false;

    #endregion

    private int curQuestCount = 0;
    private GameManager gameManager;

    private DailySlotController slotController;

    [SerializeField] private List<DailyQuestData> allQuests; // 모든 퀘스트 데이터
    private List<DailyQuestData> activeQuests;


    public void Init(GameManager gm)
    {
        gameManager = gm;
       
        slotController = GetComponentInChildren<DailySlotController>();
        activeQuests = new List<DailyQuestData>();
        RandomPickQuest();


        if (slotController != null)
        {
            slotController.Init(this);
        }
    }
   


    public void LoadQuest() //여기에서 데이터 로드해야함 
    {

    }

    public void ProgressQuest(string questId, int amount) // 외부에서 퀘스트 호출해야 증가 
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


    public void BonusQuestReward()
    {
        if (curQuestCount >= maxQuestClearCount)
        {
            GameManager.Instance.ForgeManager.AddDia(100); //다이아 100개 지급

        }

    }

    private void RandomPickQuest()
    {
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, allQuests.Count);

            if (activeQuests.Contains(allQuests[randomIndex]))
            {
                i--; // 이미 선택된 퀘스트는 다시 선택
                continue;
            }
            activeQuests.Add(allQuests[randomIndex]); //랜덤으로 퀘스트 선택
        }

    }

    public List<DailyQuestData> GetActiveQuests() => activeQuests;


}