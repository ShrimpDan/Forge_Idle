using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class QuestWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform questSlotRoot;
    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private ScrollRect scrollRect;

    private List<QuestSlot> questSlots = new();

    private const int DailyQuestCount = 5; 

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.QuestWindow));
    }

    public void OpenQuests()
    {
        // 날짜 체크 후 오늘의 의뢰가 없거나 날짜가 바뀌었으면 갱신
        if (IsDailyResetNeeded())
        {
            RefreshDailyQuests();
        }

        // 오늘의 의뢰 가져오기
        var questList = LoadTodayQuests();

        foreach (Transform child in questSlotRoot)
            Destroy(child.gameObject);

        questSlots.Clear();

        foreach (var quest in questList)
        {
            var slotObj = Instantiate(questSlotPrefab, questSlotRoot);
            var slot = slotObj.GetComponent<QuestSlot>();
            slot.Init(quest, gameManager.DataManager, uIManager, () => { /* 퀘스트 완료 후 행동 */ });
            questSlots.Add(slot);
        }

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    private bool IsDailyResetNeeded()
    {
        string savedDate = PlayerPrefs.GetString("DailyQuestDate", "");
        string today = DateTime.Now.ToString("yyyyMMdd");
        return savedDate != today;
    }

    // 오늘의 의뢰 저장
    private void SaveTodayQuests(List<QuestData> todayQuests)
    {
        var keys = new List<string>();
        foreach (var q in todayQuests)
            keys.Add(q.Key);

        PlayerPrefs.SetString("DailyQuestKeys", string.Join(",", keys));
        PlayerPrefs.SetString("DailyQuestDate", DateTime.Now.ToString("yyyyMMdd"));
        PlayerPrefs.Save();
    }

    private List<QuestData> LoadTodayQuests()
    {
        var allQuests = gameManager.DataManager.QuestLoader.QuestLists;
        string keyStr = PlayerPrefs.GetString("DailyQuestKeys", "");
        var todayQuests = new List<QuestData>();

        if (!string.IsNullOrEmpty(keyStr))
        {
            var keyArr = keyStr.Split(',');
            foreach (var key in keyArr)
            {
                var q = allQuests.Find(x => x.Key == key);
                if (q != null) todayQuests.Add(q);
            }
        }
        return todayQuests;
    }

    // 오늘의 의뢰 갱신 (랜덤)
    private void RefreshDailyQuests()
    {
        var allQuests = gameManager.DataManager.QuestLoader.QuestLists;
        var selectedQuests = GetRandomQuests(allQuests, DailyQuestCount);
        SaveTodayQuests(selectedQuests);
    }

    private List<QuestData> GetRandomQuests(List<QuestData> source, int count)
    {
        var tempList = new List<QuestData>(source);
        var result = new List<QuestData>();
        System.Random rand = new System.Random();

        for (int i = 0; i < count && tempList.Count > 0; i++)
        {
            int idx = rand.Next(tempList.Count);
            result.Add(tempList[idx]);
            tempList.RemoveAt(idx);
        }
        return result;
    }
}
