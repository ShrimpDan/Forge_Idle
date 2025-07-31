using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineResourceCollectManager : MonoBehaviour
{
    public static MineResourceCollectManager Instance;

    [Header("UI")]
    private TMP_Text minedAmountText; // �ܺο��� ����
    private List<MineGroup> mineGroups; // �ܺο��� ����
    private int currentMineGroupIndex = 0;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 1.0f;
    private float timer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            timer = 0f;
            UpdateExpectedMiningAmount();
        }
    }

    // �ܺο��� ����
    public void SetMineGroups(List<MineGroup> groups)
    {
        mineGroups = groups;
    }

    public void SetCurrentMineGroupIndex(int idx)
    {
        currentMineGroupIndex = idx;
        UpdateExpectedMiningAmount();
    }

    public void SetMinedAmountText(TMP_Text text)
    {
        Debug.Log("[Mine] SetMinedAmountText: " + (text != null ? text.name : "NULL"));
        minedAmountText = text;
    }

    // --- �ǽð� ���� ä���� ǥ��
    public void UpdateExpectedMiningAmount()
    {
        if (mineGroups == null || minedAmountText == null)
        {
            Debug.LogWarning("[Mine] mineGroups or minedAmountText is null!");
            return;
        }
        if (currentMineGroupIndex < 0 || currentMineGroupIndex >= mineGroups.Count)
        {
            Debug.LogWarning("[Mine] currentMineGroupIndex out of range!");
            return;
        }
        var group = mineGroups[currentMineGroupIndex];
        var txt = GetExpectedResourcesText(group);
        minedAmountText.text = txt;
        Debug.Log($"[Mine] UpdateExpectedMiningAmount: txt={txt}");

    }

    // --- ���� �ڿ� ����
    public void CollectResources(MineGroup group, LackPopup lackPopupPrefab = null, Transform lackpopupRoot = null, UIManager uiManager = null)
    {
        if (group == null || group.mineManager == null || group.mineManager.Mine == null)
        {
            Debug.LogError("[CollectManager] �׷� �Ǵ� ���ε����Ͱ� null");
            return;
        }

        // �ڿ� ���(����) �� ����
        var resourceDict = CalculateMiningAmount(group, useRandom: true, out float hours);

        // �ּҽð� ���� ���� �Ұ�
        if (hours <= 0.01f)
        {
            if (lackPopupPrefab != null && lackpopupRoot != null)
            {
                var popup = GameObject.Instantiate(lackPopupPrefab, lackpopupRoot);
                popup.Init(GameManager.Instance, uiManager);
                popup.ShowCustom("������ �� �ִ� �ð��� �����մϴ�.");
            }
            Debug.Log("[CollectManager] ���� ������ �ð��� ����.");
            return;
        }

        // ���� ����
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        bool gotAnyResource = false;

        foreach (var pair in resourceDict)
        {
            int amountInt = Mathf.FloorToInt(pair.Value);
            if (amountInt > 0)
            {
                ItemData item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);
                if (item != null)
                {
                    GameManager.Instance.Inventory.AddItem(item, amountInt);
                    Debug.Log($"[CollectManager] {item.Name} x {amountInt} ����!");
                    sb.AppendLine($"{item.Name} x {amountInt}");
                    gotAnyResource = true;
                }
                else
                {
                    Debug.LogWarning($"[CollectManager] �ڿ� Ű�� �ش��ϴ� ItemData�� ã�� ����: {pair.Key}");
                }
            }
        }

        // �ڿ� ����
        if (!gotAnyResource)
        {
            if (lackPopupPrefab != null && lackpopupRoot != null)
            {
                var popup = GameObject.Instantiate(lackPopupPrefab, lackpopupRoot);
                popup.Init(GameManager.Instance, uiManager);
                popup.Show(LackType.Resource);
            }
            if (minedAmountText != null)
                minedAmountText.text = ""; // Ȥ�� "���� ������ �ڿ��� �����ϴ�."
            return;
        }

        // ȹ�� ���� ǥ��
        if (minedAmountText != null)
            minedAmountText.text = sb.ToString();

        // ���� �� �ð� ����
        group.lastCollectTime = DateTime.Now;
        foreach (var slot in group.mineManager.Slots)
        {
            if (slot.IsAssigned) slot.AssignedTime = group.lastCollectTime;
        }
    }

    // --- ������ �ؽ�Ʈ (��հ� ���)
    public string GetExpectedResourcesText(MineGroup group)
    {
        var resourceDict = CalculateMiningAmount(group, useRandom: false, out _);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var pair in resourceDict)
        {
            int amountInt = Mathf.FloorToInt(pair.Value);
            if (amountInt > 0)
            {
                ItemData item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);
                if (item != null)
                    sb.AppendLine($"{item.Name} x {amountInt}");
                else
                    sb.AppendLine($"{pair.Key} x {amountInt}");
            }
        }
        return sb.ToString();
    }

    // --- ���� �ڿ� ��� (����/��� ���)
    private Dictionary<string, float> CalculateMiningAmount(MineGroup group, bool useRandom, out float hours)
    {
        hours = 0f;
        if (group == null || group.mineManager == null || group.mineManager.Mine == null)
            return new Dictionary<string, float>();

        DateTime now = DateTime.Now;
        DateTime lastCollect = group.lastCollectTime;
        hours = (float)(now - lastCollect).TotalHours;

        MineData mineData = group.mineManager.Mine;
        var resourceTypes = mineData.RewardMineralKeys ?? new List<string>();
        int minAmount = mineData.CollectMin;
        int maxAmount = mineData.CollectMax;
        float basePerHour = mineData.CollectRatePerHour;

        Dictionary<string, float> resourceDict = new Dictionary<string, float>();
        foreach (string resKey in resourceTypes)
            resourceDict[resKey] = 0f;

        // === ������� ������ �α� �߰� ===
        Debug.Log($"[Mine] CalculateMiningAmount: hours={hours}, group={group.mineKey}, slots={group.mineManager.Slots.Count}");

        foreach (var slot in group.mineManager.Slots)
        {
            Debug.Log($"[MineSlot] assigned={slot.IsAssigned}, assistant={(slot.AssignedAssistant != null ? slot.AssignedAssistant.Key : "NULL")}, assignedTime={slot.AssignedTime}");

            if (!slot.IsAssigned || slot.AssignedAssistant == null)
            {
                Debug.Log("[MineSlot] >> SKIP (not assigned)");
                continue;
            }

            AssistantInstance assistant = slot.AssignedAssistant;
            float gradeMultiplier = GetGradeMultiplier(assistant.grade);
            float specMultiplier = GetMineSpecMultiplier(assistant);

            DateTime assignedTime = slot.AssignedTime;
            float elapsedHour = Mathf.Min(hours, (float)(now - assignedTime).TotalHours);

            Debug.Log($"[MineSlot] elapsedHour={elapsedHour}, hours={hours}, now={now}, assignedTime={assignedTime}");

            if (elapsedHour <= 0f)
            {
                Debug.Log("[MineSlot] >> SKIP (elapsedHour<=0)");
                continue;
            }

            // ...���� ����
            foreach (string resKey in resourceTypes)
            {
                int amount = useRandom ? UnityEngine.Random.Range(minAmount, maxAmount + 1) : (minAmount + maxAmount) / 2;
                float resource = basePerHour * amount * gradeMultiplier * specMultiplier * elapsedHour;
                resourceDict[resKey] += resource;
            }
        }
        return resourceDict;
    }


    private float GetGradeMultiplier(string grade)
    {
        switch (grade)
        {
            case "UR": return 1.4f;
            case "SSR": return 1.3f;
            case "SR": return 1.2f;
            case "R": return 1.1f;
            case "N": return 1.0f;
            default: return 1.0f;
        }
    }

    private float GetMineSpecMultiplier(AssistantInstance assistant)
    {
        if (assistant == null || assistant.Multipliers == null) return 1.0f;
        foreach (var m in assistant.Multipliers)
        {
            if (!string.IsNullOrEmpty(m.AbilityName) &&
                (m.AbilityName.ToLower().Contains("mine") || m.AbilityName.Contains("ä��")))
            {
                return m.Multiplier;
            }
        }
        return 1.0f;
    }
}
