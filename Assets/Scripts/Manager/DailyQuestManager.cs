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

    private DailySlotController slotController;

    [SerializeField] private List<DailyQuestData> allQuests; // 모든 퀘스트 데이터
    
    
    private List<DailyQuestData> activeQuests;
    private Dictionary<string, DailyQuestLoader> activeQuestDic = new();

    [SerializeField] private Image totalQuestGauge;
    [SerializeField] private TextMeshProUGUI TotalQuestText;
    [SerializeField] private Button bounsRewardButton;




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

        if (!activeQuestDic.TryGetValue(questId, out var loader))
        {
            return;
        }
        if (loader.isCompleted)
        {
            return;
        }

        loader.currentAmount = Mathf.Min(loader.currentAmount + amount, loader.data.goalAmount);

        if (loader.isCompleted)
        {
            Debug.Log("퀘스트 완료");
        }

        RefreshUI();


    }

    public void ClaimReward(string questId)
    {

        if (!activeQuestDic.TryGetValue(questId, out var loader))
        {
            return;
        }

        if (!loader.isCompleted || loader.isClaimed)
        {
            return;
        }

        loader.isClaimed = true;
        curQuestCount++;



        gameManager.ForgeManager.AddDia(loader.data.rewardCount);

        RefreshUI();

        //모든 보상클리어시 해당 보상버튼 활성화 하는 기능 추가 

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
            int randomIndex = Random.Range(0, allQuests.Count);
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
            if (loader.isCompleted && loader.isClaimed)
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
        UpdateTotalQuestProgressUI();
    }

    public Dictionary<string, DailyQuestLoader> GetActiveQuestDic() => activeQuestDic;


}