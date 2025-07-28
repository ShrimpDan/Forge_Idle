using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DailyQuestManager : MonoBehaviour
{

    #region  모든퀘스트 관련정보
    private int maxQuestClearCount = 5;
    private bool isAllClear = false;

    #endregion

    private int curQuestCount = 0;
    private GameManager gameManager;

    [SerializeField]private DailySlotController slotController;

    [SerializeField] private List<DailyQuestData> allQuests; // 모든 퀘스트 데이터
    
    
    private List<DailyQuestData> activeQuests;
    private Dictionary<string, DailyQuestLoader> activeQuestDic = new();

    [SerializeField] private Image totalQuestGauge;
    [SerializeField] private TextMeshProUGUI TotalQuestText;
    [SerializeField] private Button bounsRewardButton;

    [SerializeField] private int TestTime;
    private bool isResetting = false;

    private const string lastDateKey= "LastQusetResetDate";

    public void Init(GameManager gm)
    {
        gameManager = gm;
       
       
        activeQuests = new List<DailyQuestData>();
        RandomPickQuest();

        Debug.Log($"랜덤 퀘스트 개수: {activeQuestDic.Count}"); // 데이터가 잘 들어갔는지 확인

        CheckDailyReset();

        if (slotController != null)
        {
            Debug.Log("SlotController Init 호출됨");
            slotController.Init(this);
        }
        else
        {
            Debug.Log("SlotController 호출 안됨");
        }
    }
   




    public void ProgressQuest(string questId, int amount) // 외부에서 퀘스트 호출해야 증가 
    {

        if (!activeQuestDic.TryGetValue(questId, out var loader))
        {
            return;
        }
        if (loader.isCompleted || !loader.isAccepted)
        {
            return;
        }
        
        loader.currentAmount = Mathf.Min(loader.currentAmount + amount, loader.data.goalAmount);     
        RefreshUI();
    }

    public void ClaimReward(string questId)
    {

        if (!activeQuestDic.TryGetValue(questId, out var loader))
        {
            return;
        }

        if (!loader.isCompleted || loader.isRewardClaimed)
        {
            return;
        }

        loader.isRewardClaimed = true;
        curQuestCount++;
        gameManager.ForgeManager.AddDia(loader.data.rewardCount);
        RefreshUI();
    }


    public void BonusQuestReward()
    {
        if (curQuestCount >= maxQuestClearCount)
        {
            isAllClear = true;
            GameManager.Instance.ForgeManager.AddDia(100);
        }

    }

    private void RandomPickQuest()
    {
        activeQuestDic.Clear();
       

        while(activeQuestDic.Count <maxQuestClearCount)
        { 
            int randomIndex = UnityEngine.Random.Range(0, allQuests.Count);
            var data = allQuests[randomIndex];

            if (activeQuestDic.ContainsKey(data.questId))
            {
                continue;
            }

            activeQuestDic.Add(data.questId, new DailyQuestLoader(data));         
        }
    }

    private void UpdateTotalQuestProgressUI()
    {
        int total = maxQuestClearCount;
        int completed = 0;

        foreach (var loader in activeQuestDic.Values)
        {
            if (loader.isCompleted && loader.isRewardClaimed)
            {
                completed++;
            }
        }

        float progress = (float)completed / total;
        totalQuestGauge.fillAmount = progress;

        if (completed >= total && !isAllClear)
        {
            TotalQuestText.text = $"완료 {completed}/{total} 보상 가능!";
            bounsRewardButton.interactable = true;
        }
        else
        {
            TotalQuestText.text = $"완료 {completed}/{total}";
            bounsRewardButton.interactable = false;
        }

    }

    public void RefreshUI()
    {
        slotController?.Refresh();
        CheckDailyReset();
        UpdateTotalQuestProgressUI();
    }

 
    private void CheckDailyReset()
    {
        if (isResetting) return; // ⭐ 초기화 중 중복 실행 방지

        string lastDateString = PlayerPrefs.GetString(lastDateKey, "");
        DateTime lastDateTime = string.IsNullOrEmpty(lastDateString)
            ? DateTime.MinValue
            : DateTime.Parse(lastDateString);

        //DateTime now = DateTime.Now; 로컬시간 사용 뺌
        DateTime now = TimeManager.Instance.Now();
        DateTime resetTime = DateTime.Today.AddHours(23).AddMinutes(TestTime).AddMinutes(0);
        //NTP서버 


        if (lastDateTime < resetTime && now >= resetTime)
        {
            isResetting = true; // ⭐ 플래그 세팅

            ResetQuests();
            PlayerPrefs.SetString(lastDateKey, now.ToString());
            PlayerPrefs.Save();

            isResetting = false;
        }
    }

    private void ResetQuests()
    {
        activeQuestDic.Clear();
        curQuestCount = 0;
        isAllClear = false;

        RandomPickQuest();   // 새로운 퀘스트 뽑기
        RefreshUI();
    }

    public Dictionary<string, DailyQuestLoader> GetActiveQuestDic() => activeQuestDic;


}