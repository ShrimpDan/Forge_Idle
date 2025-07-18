//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System;
//using System.Collections.Generic;

//public class QuestSlot : MonoBehaviour
//{
//    [Header("UI")]
//    [SerializeField] private Image rewardIcon;
//    [SerializeField] private TMP_Text rewardAmountText;
//    [SerializeField] private TMP_Text questNameText;
//    [SerializeField] private TMP_Text timeLeftText;
//    [SerializeField] private Transform starsRoot;
//    [SerializeField] private Sprite starFilled;
//    [SerializeField] private Sprite starEmpty;
//    [SerializeField] private Transform assistantsRoot;
//    [SerializeField] private GameObject assistantSlotPrefab;
//    [SerializeField] private GameObject completeQuest;
//    [SerializeField] private Button completeButton;
//    [SerializeField] private GameObject completeImage;

//    private QuestData questData;
//    private List<AssistantInstance> assignedAssistants = new();
//    private List<GameObject> assistantSlots = new();
//    private float timer;
//    private bool isRunning;
//    private bool questCompleted = false;
//    private Action onQuestCompleted;
//    private QuestProgressData progress;

//    private string progressTimeKey => $"quest_progress_time_{questData.Key}";

//    public void Init(QuestData data, DataManager dataManager, UIManager uiManager, Action onQuestCompleted = null)
//    {
//        questData = data;
//        this.onQuestCompleted = onQuestCompleted;
//        assignedAssistants.Clear();
//        assistantSlots.Clear();
//        isRunning = false;
//        questCompleted = false;

//        if (dataManager.ItemLoader != null && questData.RewardItemKeys != null && questData.RewardItemKeys.Count > 0)
//        {
//            var itemData = dataManager.ItemLoader.GetItemByKey(questData.RewardItemKeys[0]);
//            rewardIcon.sprite = itemData != null ? IconLoader.GetIconByPath(itemData.IconPath) : null;
//        }
//        rewardAmountText.text = $"{questData.RewardMin}~{questData.RewardMax}";
//        questNameText.text = questData.QuestName;
//        UpdateStarsUI(questData.Difficulty);

//        // 저장 데이터 불러오기 
//        progress = QuestProgressManager.Load(questData.Key) ?? new QuestProgressData();
//        timer = progress.Timer > 0 ? progress.Timer : questData.Duration;
//        isRunning = progress.IsRunning;
//        questCompleted = progress.IsCompleted;

//        // 어시스턴트 복원 (인스턴스 직접 저장/복원)
//        assignedAssistants = new List<AssistantInstance>(progress.AssignedAssistants ?? new List<AssistantInstance>());

//        while (assignedAssistants.Count < questData.RequiredAssistants)
//            assignedAssistants.Add(null);
//        if (assignedAssistants.Count > questData.RequiredAssistants)
//            assignedAssistants.RemoveRange(questData.RequiredAssistants, assignedAssistants.Count - questData.RequiredAssistants);

//        // 시간 복원 (경과 적용)
//        if (isRunning && !questCompleted)
//        {
//            string lastTimeStr = PlayerPrefs.GetString(progressTimeKey, DateTime.Now.ToString());
//            DateTime lastTime = DateTime.Parse(lastTimeStr);
//            float elapsed = (float)(DateTime.Now - lastTime).TotalSeconds;
//            timer = Mathf.Max(0, timer - elapsed);

//            if (timer <= 0)
//            {
//                isRunning = false;
//                questCompleted = true;
//                progress.IsRunning = false;
//                progress.IsCompleted = true;
//                progress.Timer = 0;
//                QuestProgressManager.Save(questData.Key, progress);
//            }
//        }

//        // UI 어시스턴트 슬롯 생성/복원 
//        foreach (Transform child in assistantsRoot)
//            Destroy(child.gameObject);
//        assistantSlots.Clear();
//        for (int i = 0; i < questData.RequiredAssistants; i++)
//        {
//            var slot = Instantiate(assistantSlotPrefab, assistantsRoot);
//            int idx = i;
//            slot.GetComponent<Button>().onClick.RemoveAllListeners();
//            slot.GetComponent<Button>().onClick.AddListener(() => OnClickAssistantSlot(idx, dataManager, uiManager));
//            SetAssistantSlot(slot, assignedAssistants[idx]);
//            assistantSlots.Add(slot);
//        }

//        if (completeQuest != null) completeQuest.SetActive(questCompleted);
//        if (completeButton != null)
//        {
//            completeButton.gameObject.SetActive(questCompleted && !progress.RewardReceived);
//            completeButton.onClick.RemoveAllListeners();
//            completeButton.onClick.AddListener(OnClickCompleteButton);
//        }
//        if (completeImage != null)
//            completeImage.SetActive(questCompleted && progress.RewardReceived);

//        UpdateTimeUI();
//    }

//    private void OnClickAssistantSlot(int idx, DataManager dataManager, UIManager uiManager)
//    {
//        if (isRunning || questCompleted) return;

//        var popup = uiManager.OpenUI<AssistantSelectPopup>(UIName.AssistantSelectPopup);
//        popup.Init(GameManager.Instance, uiManager);
//        popup.OpenForSelection(assistant =>
//        {
//            if (assistant == null) return;

//            var assiPopup = uiManager.OpenUI<Mine_AssistantPopup>(UIName.Mine_AssistantPopup);
//            assiPopup.Init(GameManager.Instance, uiManager);
//            assiPopup.SetAssistant(assistant, false, (selected, isAssign) =>
//            {
//                if (isAssign)
//                {
//                    assignedAssistants[idx] = selected;
//                    SetAssistantSlot(assistantSlots[idx], selected);

//                    // 인스턴스 직접 저장
//                    progress.AssignedAssistants = new List<AssistantInstance>(assignedAssistants);
//                    QuestProgressManager.Save(questData.Key, progress);

//                    if (assignedAssistants.TrueForAll(a => a != null))
//                        StartQuest();
//                }
//            });
//        }, true);
//    }

//    private void SetAssistantSlot(GameObject slotObj, AssistantInstance assistant)
//    {
//        var icon = slotObj.transform.Find("Icon")?.GetComponent<Image>();
//        var plus = slotObj.transform.Find("Plus");
//        if (icon != null)
//        {
//            if (assistant != null)
//            {
//                string iconPath = assistant?.IconPath;
//                icon.sprite = !string.IsNullOrEmpty(iconPath) ? IconLoader.GetIconByPath(iconPath) : null;
//                icon.enabled = icon.sprite != null;
//            }
//            else
//            {
//                icon.sprite = null;
//                icon.enabled = false;
//            }
//        }
//        if (plus != null)
//            plus.gameObject.SetActive(assistant == null);
//    }

//    private void UpdateStarsUI(int difficulty)
//    {
//        if (starsRoot == null || starFilled == null || starEmpty == null) return;
//        int filledCount = Mathf.Clamp(difficulty + 1, 1, 5);
//        for (int i = 0; i < starsRoot.childCount; i++)
//        {
//            var star = starsRoot.GetChild(i).GetComponent<Image>();
//            if (star == null) continue;
//            star.sprite = (i < filledCount) ? starFilled : starEmpty;
//            star.color = Color.white;
//        }
//    }

//    private void StartQuest()
//    {
//        isRunning = true;
//        timer = questData.Duration;
//        questCompleted = false;
//        if (completeQuest != null) completeQuest.SetActive(false);
//        if (completeButton != null) completeButton.gameObject.SetActive(false);
//        if (completeImage != null) completeImage.SetActive(false);

//        progress.IsRunning = true;
//        progress.IsCompleted = false;
//        progress.RewardReceived = false;
//        progress.Timer = timer;
//        progress.AssignedAssistants = new List<AssistantInstance>(assignedAssistants);
//        QuestProgressManager.Save(questData.Key, progress);
//        PlayerPrefs.SetString(progressTimeKey, DateTime.Now.ToString());
//        PlayerPrefs.Save();

//        UpdateTimeUI();
//    }

//    private void Update()
//    {
//        if (!isRunning || questCompleted) return;
//        timer -= Time.deltaTime;
//        if (timer < 0) timer = 0;
//        UpdateTimeUI();

//        if (timer <= 0 && !questCompleted)
//        {
//            isRunning = false;
//            questCompleted = true;
//            progress.IsRunning = false;
//            progress.IsCompleted = true;
//            progress.Timer = 0;
//            QuestProgressManager.Save(questData.Key, progress);

//            if (completeQuest != null) completeQuest.SetActive(true);
//            if (completeButton != null) completeButton.gameObject.SetActive(true);
//            if (completeImage != null) completeImage.SetActive(false);
//        }
//    }

//    private void OnClickCompleteButton()
//    {
//        if (!questCompleted || progress.RewardReceived) return;
//        CollectReward();
//        progress.RewardReceived = true;
//        QuestProgressManager.Save(questData.Key, progress);
//        if (completeButton != null)
//            completeButton.gameObject.SetActive(false);
//        if (completeImage != null)
//            completeImage.SetActive(true);
//    }

//    private void CollectReward()
//    {
//        if (!questCompleted) return;
//        int rewardCnt = UnityEngine.Random.Range(questData.RewardMin, questData.RewardMax + 1);
//        foreach (var key in questData.RewardItemKeys)
//        {
//            var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(key);
//            if (item != null)
//                GameManager.Instance.Inventory.AddItem(item, rewardCnt);
//        }
//        onQuestCompleted?.Invoke();
//    }

//    private void UpdateTimeUI()
//    {
//        if (timeLeftText == null) return;
//        if (questCompleted)
//            timeLeftText.text = "완료!";
//        else
//            timeLeftText.text = FormatTime(timer);
//    }

//    private string FormatTime(float sec)
//    {
//        int m = Mathf.FloorToInt(sec / 60);
//        int s = Mathf.FloorToInt(sec % 60);
//        return $"{m:00}:{s:00}";
//    }
//}
