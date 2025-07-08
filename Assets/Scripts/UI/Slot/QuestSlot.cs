using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class QuestSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text timeLeftText;
    [SerializeField] private Transform starsRoot;
    [SerializeField] private Sprite starFilled;
    [SerializeField] private Sprite starEmpty;
    [SerializeField] private Transform assistantsRoot;
    [SerializeField] private GameObject assistantSlotPrefab;
    [SerializeField] private GameObject completeQuest;      // 완료 패널
    [SerializeField] private Button completeButton;         // 완료 버튼
    [SerializeField] private GameObject completeImage;      // 완료 이미지

    private QuestData questData;
    private List<AssistantInstance> assignedAssistants = new();
    private List<GameObject> assistantSlots = new();
    private float timer;
    private bool isRunning;
    private bool questCompleted = false;
    private Action onQuestCompleted;

    public void Init(QuestData data, DataManager dataManager, UIManager uiManager, Action onQuestCompleted = null)
    {
        questData = data;
        this.onQuestCompleted = onQuestCompleted;
        assignedAssistants.Clear();
        assistantSlots.Clear();
        isRunning = false;
        questCompleted = false;
        timer = questData.Duration;

        // 리워드 표시
        if (dataManager.ItemLoader != null && questData.RewardItemKeys != null && questData.RewardItemKeys.Count > 0)
        {
            var itemData = dataManager.ItemLoader.GetItemByKey(questData.RewardItemKeys[0]);
            rewardIcon.sprite = itemData != null ? IconLoader.GetIcon(itemData.IconPath) : null;
        }
        rewardAmountText.text = $"{questData.RewardMin}~{questData.RewardMax}";
        questNameText.text = questData.QuestName;

        // 별 UI 갱신
        UpdateStarsUI(questData.Difficulty);

        // 시간 표시
        timeLeftText.text = FormatTime(questData.Duration);

        // 어시스턴트 슬롯 초기화
        foreach (Transform child in assistantsRoot)
            Destroy(child.gameObject);
        for (int i = 0; i < questData.RequiredAssistants; i++)
        {
            var slot = Instantiate(assistantSlotPrefab, assistantsRoot);
            int idx = i;
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.GetComponent<Button>().onClick.AddListener(() => OnClickAssistantSlot(idx, dataManager, uiManager));
            SetAssistantSlot(slot, null);
            assistantSlots.Add(slot);
            assignedAssistants.Add(null);
        }

        // 완료 패널/버튼/이미지 상태 초기화
        if (completeQuest != null) completeQuest.SetActive(false);
        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(false);
            completeButton.onClick.RemoveAllListeners();
            completeButton.onClick.AddListener(OnClickCompleteButton);
        }
        if (completeImage != null) completeImage.SetActive(false);
    }

    private void OnClickAssistantSlot(int idx, DataManager dataManager, UIManager uiManager)
    {
        if (isRunning || questCompleted) return;
        var popup = uiManager.OpenUI<AssistantSelectPopup>(UIName.AssistantSelectPopup);
        popup.Init(GameManager.Instance, uiManager);
        popup.OpenForSelection(assistant =>
        {
            if (assistant == null) return;
            assignedAssistants[idx] = assistant;
            SetAssistantSlot(assistantSlots[idx], assistant);
            if (assignedAssistants.TrueForAll(a => a != null))
                StartQuest();
        });
    }

    private void SetAssistantSlot(GameObject slotObj, AssistantInstance assistant)
    {
        var icon = slotObj.transform.Find("Icon")?.GetComponent<Image>();
        var plus = slotObj.transform.Find("Plus");
        if (icon != null)
        {
            if (assistant != null)
            {
                string iconPath = assistant?.IconPath;
                icon.sprite = !string.IsNullOrEmpty(iconPath) ? IconLoader.GetIcon(iconPath) : null;
                icon.enabled = icon.sprite != null;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
            }
        }
        if (plus != null)
            plus.gameObject.SetActive(assistant == null);
    }

    private void UpdateStarsUI(int difficulty)
    {
        if (starsRoot == null || starFilled == null || starEmpty == null) return;
        int filledCount = Mathf.Clamp(difficulty + 1, 1, 5);
        for (int i = 0; i < starsRoot.childCount; i++)
        {
            var star = starsRoot.GetChild(i).GetComponent<Image>();
            if (star == null) continue;
            star.sprite = (i < filledCount) ? starFilled : starEmpty;
            star.color = Color.white;
        }
    }

    private void StartQuest()
    {
        isRunning = true;
        timer = questData.Duration;
        if (completeQuest != null) completeQuest.SetActive(false);
        if (completeButton != null) completeButton.gameObject.SetActive(false);
        if (completeImage != null) completeImage.SetActive(false);
        UpdateTimeUI();
    }

    private void Update()
    {
        if (!isRunning || questCompleted) return;
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        UpdateTimeUI();

        if (timer <= 0 && !questCompleted)
        {
            isRunning = false;
            questCompleted = true;

            // 완료 패널 세팅
            if (completeQuest != null) completeQuest.SetActive(true);
            if (completeButton != null) completeButton.gameObject.SetActive(true);
            if (completeImage != null) completeImage.SetActive(false);
        }
    }

    private void OnClickCompleteButton()
    {
        // 보상 지급
        CollectReward();
        // 버튼만 끄고, 이미지 키기
        if (completeButton != null)
            completeButton.gameObject.SetActive(false);
        if (completeImage != null)
            completeImage.SetActive(true);
    }

    private void CollectReward()
    {
        if (!questCompleted) return;
        int rewardCnt = UnityEngine.Random.Range(questData.RewardMin, questData.RewardMax + 1);
        foreach (var key in questData.RewardItemKeys)
        {
            var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(key);
            if (item != null)
                GameManager.Instance.Inventory.AddItem(item, rewardCnt);
        }
        onQuestCompleted?.Invoke();
    }

    private void UpdateTimeUI()
    {
        if (timeLeftText == null) return;
        if (questCompleted)
        {
            timeLeftText.text = "완료!";
        }
        else
        {
            timeLeftText.text = FormatTime(timer);
        }
    }

    private string FormatTime(float sec)
    {
        int m = Mathf.FloorToInt(sec / 60);
        int s = Mathf.FloorToInt(sec % 60);
        return $"{m:00}:{s:00}";
    }
}
