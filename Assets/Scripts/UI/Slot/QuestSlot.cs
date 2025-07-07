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
    [SerializeField] private Transform starsRoot;           // ⭐ 별 5개 루트
    [SerializeField] private Sprite starFilled;             // 채워진 별 스프라이트
    [SerializeField] private Sprite starEmpty;              // 빈 별 스프라이트
    [SerializeField] private Transform assistantsRoot;
    [SerializeField] private GameObject assistantSlotPrefab;
    [SerializeField] private GameObject completeQuest;      // 완료 오브젝트 (버튼 포함)
    [SerializeField] private Button completeButton;         // 완료 버튼

    private QuestData questData;
    private List<AssistantInstance> assignedAssistants = new();
    private List<GameObject> assistantSlots = new();
    private float timer;
    private bool isRunning;
    private Action onQuestCompleted;
    private bool questCompleted = false;

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

        UpdateStarsUI(questData.Difficulty);

        // 시간
        timeLeftText.text = FormatTime(questData.Duration);

        // 어시스턴트 슬롯 정리 및 생성
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

        // 완료 UI
        if (completeQuest != null) completeQuest.SetActive(false);
        if (completeButton != null)
        {
            completeButton.onClick.RemoveAllListeners();
            completeButton.onClick.AddListener(CollectReward);
        }
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

    // 어시스턴트 슬롯 UI 아이콘 변경
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

    // 별 난이도 UI
    private void UpdateStarsUI(int difficulty)
    {
        if (starsRoot == null || starFilled == null || starEmpty == null) return;
        int filledCount = Mathf.Clamp(difficulty + 1, 1, 5); // 0=별1개, 4=별5개

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
            // 시간 그대로 멈추고 완료UI만 활성화
            if (completeQuest != null) completeQuest.SetActive(true);
        }
    }

    private void CollectReward()
    {
        // 이미 완료 처리했다면 중복 지급 방지
        if (!questCompleted) return;
        int rewardCnt = UnityEngine.Random.Range(questData.RewardMin, questData.RewardMax + 1);
        foreach (var key in questData.RewardItemKeys)
        {
            var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(key);
            if (item != null)
                GameManager.Instance.Inventory.AddItem(item, rewardCnt);
        }
        if (completeQuest != null) completeQuest.SetActive(false);
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
