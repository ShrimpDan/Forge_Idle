using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineResourceCollectManager : MonoBehaviour
{
    public static MineResourceCollectManager Instance;

    private TMP_Text minedAmountText;
    private List<MineGroup> mineGroups;
    private int currentMineGroupIndex = 0;

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
        minedAmountText = text;
    }

    public void UpdateExpectedMiningAmount()
    {
        if (mineGroups == null || minedAmountText == null) return;
        if (currentMineGroupIndex < 0 || currentMineGroupIndex >= mineGroups.Count) return;
        var group = mineGroups[currentMineGroupIndex];
        minedAmountText.text = GetExpectedResourcesText(group);
    }

    public void CollectResources(MineGroup group, LackPopup lackPopupPrefab = null, Transform lackpopupRoot = null, UIManager uiManager = null)
    {
        if (group == null || group.mineManager == null || group.mineManager.Mine == null) return;

        var resourceDict = CalculateMiningAmount(group, useRandom: true, out float hours);

        if (hours <= 0.01f)
        {
            if (lackPopupPrefab != null && lackpopupRoot != null)
            {
                var popup = GameObject.Instantiate(lackPopupPrefab, lackpopupRoot);
                popup.Init(GameManager.Instance, uiManager);
                popup.ShowCustom("수령할 수 있는 시간이 부족합니다.");
            }
            if (minedAmountText != null)
                minedAmountText.text = "";
            return;
        }

        List<(ItemData item, int count)> rewardList = new List<(ItemData, int)>();
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
                    sb.AppendLine($"{item.Name} x {amountInt}");
                    rewardList.Add((item, amountInt));
                }
            }
        }

        if (rewardList.Count == 0)
        {
            if (lackPopupPrefab != null && lackpopupRoot != null)
            {
                var popup = GameObject.Instantiate(lackPopupPrefab, lackpopupRoot);
                popup.Init(GameManager.Instance, uiManager);
                popup.Show(LackType.Resource);
            }
            if (minedAmountText != null)
                minedAmountText.text = "";
            return;
        }

        var popupObj = GameObject.Find("RewardPopup");
        if (popupObj != null)
        {
            var popup = popupObj.GetComponent<RewardPopup>();
            if (popup != null)
                popup.Show(ToRewardDict(rewardList), "획득 보상");
        }

        if (minedAmountText != null)
            minedAmountText.text = sb.ToString();

        group.lastCollectTime = DateTime.Now;
        foreach (var slot in group.mineManager.Slots)
            if (slot.IsAssigned) slot.AssignedTime = group.lastCollectTime;
    }

    private Dictionary<ItemData, int> ToRewardDict(List<(ItemData item, int count)> list)
    {
        var dict = new Dictionary<ItemData, int>();
        foreach (var t in list)
        {
            if (t.item == null) continue;
            if (dict.ContainsKey(t.item)) dict[t.item] += t.count;
            else dict.Add(t.item, t.count);
        }
        return dict;
    }

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
                sb.AppendLine(item != null
                    ? $"{item.Name} x {amountInt}"
                    : $"{pair.Key} x {amountInt}");
            }
        }
        return sb.ToString();
    }

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

        foreach (var slot in group.mineManager.Slots)
        {
            if (!slot.IsAssigned || slot.AssignedAssistant == null)
                continue;

            AssistantInstance assistant = slot.AssignedAssistant;
            float gradeMultiplier = GetGradeMultiplier(assistant.grade);
            float specMultiplier = GetMineSpecMultiplier(assistant);

            DateTime assignedTime = slot.AssignedTime;
            float elapsedHour = Mathf.Min(hours, (float)(now - assignedTime).TotalHours);

            if (elapsedHour <= 0f)
                continue;

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
                (m.AbilityName.ToLower().Contains("mine") || m.AbilityName.Contains("채광")))
                return m.Multiplier;
        }
        return 1.0f;
    }
}

