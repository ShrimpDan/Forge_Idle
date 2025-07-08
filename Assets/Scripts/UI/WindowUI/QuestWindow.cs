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

        GenerateQuestSlots();
    }

    private void GenerateQuestSlots()
    {
        Debug.Log("GenerateQuestSlots 호출됨");
        Debug.Log(gameManager == null ? "gameManager null!" : "gameManager 연결됨");
        Debug.Log(gameManager?.DataManager == null ? "DataManager null!" : "DataManager 연결됨");
        Debug.Log(gameManager?.DataManager?.QuestLoader == null ? "QuestLoader null!" : "QuestLoader 연결됨");

        if (gameManager == null || gameManager.DataManager == null || gameManager.DataManager.QuestLoader == null)
        {
            Debug.LogError("필수 참조 누락! QuestSlot 생성 불가!");
            return;
        }

        if (IsDailyResetNeeded())
            RefreshDailyQuests();

        var questList = LoadTodayQuests();

        if (questList == null || questList.Count == 0)
        {
            Debug.LogWarning("[QuestWindow] 오늘의 의뢰가 없어 새로 갱신합니다.");
            RefreshDailyQuests();
            questList = LoadTodayQuests();
        }

        Debug.Log($"슬롯 생성할 퀘스트 개수: {questList.Count}");

        foreach (Transform child in questSlotRoot)
            Destroy(child.gameObject);

        questSlots.Clear();

        foreach (var quest in questList)
        {
            var slotObj = Instantiate(questSlotPrefab, questSlotRoot);
            var slot = slotObj.GetComponent<QuestSlot>();
            slot.Init(quest, gameManager.DataManager, uIManager, () => { });
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
