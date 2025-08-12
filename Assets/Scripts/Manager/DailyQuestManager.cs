using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestManager : MonoBehaviour
{
    #region 제이슨 저장정보

    private const string SaveFileName = "dailyQuest.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    #endregion


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

    private DateTime lastResetTime;

    private const string lastDateKey= "LastQuestResetDate";
    private DateTime resetTime;

    public void Init(GameManager gm)
    {
        gameManager = gm;
        slotController?.Init(this);

        if (LoadQuests())
        {
            // 하루 지났으면 퀘스트 리셋
            if (IsResetNeeded())
            {
                ResetQuests(); // 수정
            }
        }
        else
        {
            // 저장된 데이터 없으면 새 퀘스트 생성
            ResetQuests(); // 수정
        }

    }

    private void Start()
    {
        resetTime = TimeManager.Instance.Now().AddSeconds(TestTime); //일단 작동하는지 확인부터 
        bounsRewardButton.onClick.AddListener(BonusQuestReward);
    }


    private void Update()
    {
        if (IsResetNeeded()) // 하루가 지났으면 리셋
        {
            ResetQuests();
        }
    }

    public void SaveQuests()
    {
        DailyQuestSaveWrapper wrapper = new DailyQuestSaveWrapper();
        wrapper.lastResetTime = lastResetTime.ToString(); //시간 저장


        foreach (var loader in activeQuestDic.Values)
        {
            wrapper.quests.Add(new DailyQuestSaveData
            {
                questId = loader.data.questId,
                currentAmount = loader.currentAmount,
                isAccepted = loader.isAccepted,
                isRewardClaimed = loader.isRewardClaimed

            });

        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
    }

    private bool IsResetNeeded()
    {
        DateTime now = TimeManager.Instance.Now();
        return now.Date > lastResetTime.Date;
    }


    public void ProgressQuest(string questId, int amount) // 외부에서 퀘스트 호출해야 증가 
    {

        if (!activeQuestDic.TryGetValue(questId, out var loader))
        {
            return;
        }
        if (loader.isCompleted)
        {
            return;
        }

        loader.currentAmount = Mathf.Min(loader.currentAmount + amount, loader.data.goalAmount);
        RefreshUI();
    }

    private bool LoadQuests()
    {
        if (!File.Exists(SavePath))
            return false;

        string json = File.ReadAllText(SavePath);
        DailyQuestSaveWrapper wrapper = JsonUtility.FromJson<DailyQuestSaveWrapper>(json);
        if (wrapper == null)
        { 
            return false;
        }

        activeQuestDic.Clear();
        foreach (var saved in wrapper.quests)
        {
            var data = allQuests.FirstOrDefault(q => q.questId == saved.questId);
            if (data == null) continue;

            DailyQuestLoader loader = new DailyQuestLoader(data)
            {
                
                
                currentAmount = saved.currentAmount,
                isAccepted = saved.isAccepted,
                isRewardClaimed = saved.isRewardClaimed
            };
            activeQuestDic.Add(saved.questId, loader);
        }

        lastResetTime = DateTime.Parse(wrapper.lastResetTime);
        return true;
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

        SaveQuests();
    }


    public void BonusQuestReward()
    {
        if (curQuestCount >= maxQuestClearCount)
        {
            isAllClear = true;
            gameManager.ForgeManager.AddDia(100);
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
        else if (!isAllClear)
        {
            TotalQuestText.text = $"완료 {completed}/{total}";
            bounsRewardButton.interactable = false;
        }
        else
        {
            // 이미 최종 보상을 받은 상태
            TotalQuestText.text = $"모든 퀘스트 완료!";
            bounsRewardButton.interactable = false;
        }

    }

    public void RefreshUI()
    {
        slotController?.Refresh();
        UpdateTotalQuestProgressUI();
    }
      

    private void ResetQuests()
    {
        activeQuestDic.Clear();
        curQuestCount = 0;
        isAllClear = false;

        RandomPickQuest();
        lastResetTime = TimeManager.Instance.Now();
        SaveQuests();
        RefreshUI();
    }

    public Dictionary<string, DailyQuestLoader> GetActiveQuestDic() => activeQuestDic;


}