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
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text timeLeftText;
    [SerializeField] private Transform assistantsRoot;
    [SerializeField] private GameObject assistantSlotPrefab;
    [SerializeField] private Button collectButton;

    private QuestData questData;
    private List<AssistantInstance> assignedAssistants = new();
    private List<GameObject> assistantSlots = new();
    private float timer;
    private bool isRunning;
    private Action onQuestCompleted;

    public void Init(QuestData data, DataManager dataManager, UIManager uiManager, Action onQuestCompleted = null)
    {
        questData = data;
        this.onQuestCompleted = onQuestCompleted;
        assignedAssistants.Clear();
        assistantSlots.Clear();
        isRunning = false;
        timer = questData.Duration;

        // ���� ������ ǥ��
        if (dataManager.ItemLoader != null && questData.RewardItemKeys != null && questData.RewardItemKeys.Count > 0)
        {
            var itemData = dataManager.ItemLoader.GetItemByKey(questData.RewardItemKeys[0]);
            rewardIcon.sprite = itemData != null ? IconLoader.GetIcon(itemData.IconPath) : null;
        }
        rewardAmountText.text = $"{questData.RewardMin}~{questData.RewardMax}";
        questNameText.text = questData.QuestName;
        difficultyText.text = $"���̵� {questData.Difficulty + 1}";
        timeLeftText.text = $"{FormatTime(questData.Duration)}";

        // ���� ���� ����
        foreach (Transform child in assistantsRoot)
            Destroy(child.gameObject);

        // �ʿ� ���� ����
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

        collectButton.onClick.RemoveAllListeners();
        collectButton.gameObject.SetActive(false);
        collectButton.onClick.AddListener(CollectReward);
    }

    private void OnClickAssistantSlot(int idx, DataManager dataManager, UIManager uiManager)
    {
        if (isRunning) return;

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

    // **�߿�: ������ ��θ� IconPath�� ����**
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

    private void StartQuest()
    {
        isRunning = true;
        timer = questData.Duration;
        collectButton.gameObject.SetActive(false);
        UpdateTimeUI();
    }

    private void Update()
    {
        if (!isRunning) return;
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        UpdateTimeUI();

        if (timer <= 0)
        {
            isRunning = false;
            timeLeftText.text = "<color=yellow>�Ϸ�!</color>";
            collectButton.gameObject.SetActive(true);
        }
    }

    private void CollectReward()
    {
        int rewardCnt = UnityEngine.Random.Range(questData.RewardMin, questData.RewardMax + 1);
        foreach (var key in questData.RewardItemKeys)
        {
            var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(key);
            if (item != null)
                GameManager.Instance.Inventory.AddItem(item, rewardCnt);
        }
        collectButton.gameObject.SetActive(false);
        onQuestCompleted?.Invoke();
    }

    private void UpdateTimeUI()
    {
        timeLeftText.text = isRunning ? FormatTime(timer) : FormatTime(questData.Duration);
    }

    private string FormatTime(float sec)
    {
        int m = Mathf.FloorToInt(sec / 60);
        int s = Mathf.FloorToInt(sec % 60);
        return $"{m:00}:{s:00}";
    }
}
