using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineResourceCollectManager : MonoBehaviour
{
    public static MineResourceCollectManager Instance;

    [Header("UI")]
    public TMP_Text minedAmountText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void CollectResources(MineGroup group)
    {
        if (group == null || group.mineManager == null || group.mineManager.Mine == null)
        {
            Debug.LogError("[CollectManager] 그룹 또는 마인데이터가 null");
            return;
        }

        DateTime now = DateTime.Now;
        DateTime lastCollect = group.lastCollectTime;
        float hours = (float)(now - lastCollect).TotalHours;
        if (hours <= 0f)
        {
            Debug.Log("[CollectManager] 수령 가능한 시간이 없음.");
            return;
        }

        MineData mineData = group.mineManager.Mine;

        var resourceTypes = mineData.RewardMineralKeys ?? new List<string>();
        int minAmount = mineData.CollectMin;
        int maxAmount = mineData.CollectMax;
        float basePerHour = mineData.CollectRatePerHour;

        Dictionary<string, float> resourceDict = new Dictionary<string, float>();
        foreach (string resKey in resourceTypes)
        {
            resourceDict[resKey] = 0f; // 누적
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

        // 누적 획득 결과 저장용 StringBuilder
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var pair in resourceDict)
        {
            int amountInt = Mathf.FloorToInt(pair.Value);
            if (amountInt > 0)
            {
                ItemData item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);
                if (item != null)
                {
                    GameManager.Instance.Inventory.AddItem(item, amountInt);
                    Debug.Log($"[CollectManager] {item.Name} x {amountInt} 수령!");
                    sb.AppendLine($"{item.Name} x {amountInt}"); // ★ 문자열 누적
                }
                else
                {
                    Debug.LogWarning($"[CollectManager] 자원 키에 해당하는 ItemData를 찾지 못함: {pair.Key}");
                }
            }
        }

        // Text로 획득내역 표시
        if (minedAmountText != null)
        {
            if (sb.Length > 0)
                minedAmountText.text = sb.ToString();
            else
                minedAmountText.text = "수령 가능한 자원이 없습니다.";
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
            // 배율 적용
            if (!string.IsNullOrEmpty(m.AbilityName) &&
                (m.AbilityName.ToLower().Contains("mine") || m.AbilityName.Contains("채광")))
            {
                return m.Multiplier;
            }
        }
        return 1.0f;
    }
}
