using System;
using System.Collections.Generic;
using UnityEngine;

public class MineResourceCollectManager : MonoBehaviour
{
    public static MineResourceCollectManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void CollectResources(MineGroup group)
    {
        if (group == null || group.mineManager == null || group.mineManager.Mine == null)
        {
            Debug.LogError("[CollectManager] �׷� �Ǵ� ���ε����Ͱ� null");
            return;
        }

        DateTime now = DateTime.Now;
        DateTime lastCollect = group.lastCollectTime;
        float hours = (float)(now - lastCollect).TotalHours;
        if (hours <= 0f)
        {
            Debug.Log("[CollectManager] ���� ������ �ð��� ����.");
            return;
        }

        MineData mineData = group.mineManager.Mine;

        // ���� �ʵ�� ���� ����
        var resourceTypes = mineData.RewardMineralKeys ?? new List<string>();
        int minAmount = mineData.CollectMin;
        int maxAmount = mineData.CollectMax;
        float basePerHour = mineData.CollectRatePerHour;

        Dictionary<string, float> resourceDict = new Dictionary<string, float>();
        foreach (string resKey in resourceTypes)
        {
            resourceDict[resKey] = 0f; // ����
        }

        foreach (var slot in group.mineManager.Slots)
        {
            if (!slot.IsAssigned || slot.AssignedAssistant == null) continue;

            AssistantInstance assistant = slot.AssignedAssistant;
            float gradeMultiplier = GetGradeMultiplier(assistant.grade);
            float specMultiplier = GetMineSpecMultiplier(assistant);

            DateTime assignedTime = slot.AssignedTime;
            float elapsedHour = Mathf.Min(hours, (float)(now - assignedTime).TotalHours);
            if (elapsedHour <= 0f) continue;

            foreach (string resKey in resourceTypes)
            {
                int randomAmount = UnityEngine.Random.Range(minAmount, maxAmount + 1);
                float amount = basePerHour * randomAmount * gradeMultiplier * specMultiplier * elapsedHour;
                resourceDict[resKey] += amount;
            }
        }

        // ���� �κ��丮 �߰� �� �α�
        foreach (var pair in resourceDict)
        {
            int amountInt = Mathf.FloorToInt(pair.Value);
            if (amountInt > 0)
            {
                // DataManager �ν��Ͻ� ����
                ItemData item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);
                if (item != null)
                {
                    GameManager.Instance.Inventory.AddItem(item, amountInt);
                    Debug.Log($"[CollectManager] {item.Name} x {amountInt} ����!");
                }
                else
                {
                    Debug.LogWarning($"[CollectManager] �ڿ� Ű�� �ش��ϴ� ItemData�� ã�� ����: {pair.Key}");
                }
            }
        }

        group.lastCollectTime = now;
        foreach (var slot in group.mineManager.Slots)
        {
            if (slot.IsAssigned) slot.AssignedTime = now;
        }
    }

    float GetGradeMultiplier(string grade)
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

    float GetMineSpecMultiplier(AssistantInstance assistant)
    {
        if (assistant == null || assistant.Multipliers == null) return 1.0f;
        foreach (var m in assistant.Multipliers)
        {
            // ���� ����
            if (!string.IsNullOrEmpty(m.AbilityName) &&
                (m.AbilityName.ToLower().Contains("mine") || m.AbilityName.Contains("ä��")))
            {
                return m.Multiplier;
            }
        }
        return 1.0f;
    }
}
