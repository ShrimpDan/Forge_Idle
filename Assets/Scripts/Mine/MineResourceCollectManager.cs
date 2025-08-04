using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class MineResourceCollectManager : MonoBehaviour
{
    public static MineResourceCollectManager Instance;

    [Header("UI Roots & Prefabs")]
    [SerializeField] private Transform amountRoot;
    [SerializeField] private GameObject itemAmountSlotPrefab;

    private List<MineGroup> mineGroups;
    private int currentMineGroupIndex = 0;

    [SerializeField] private float updateInterval = 1.0f;
    private float timer = 0f;

    [Header("UI Prefabs (Inspector에서 할당 필수!)")]
    [SerializeField] private GameObject rewardPopupPrefab;
    [SerializeField] private LackPopup lackPopupPrefab;
    [SerializeField] private Transform lackpopupRoot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateExpectedMiningAmount();
        }
    }

    public void SetMineGroups(List<MineGroup> groups) => mineGroups = groups;

    public void SetCurrentMineGroupIndex(int idx)
    {
        currentMineGroupIndex = idx;
        UpdateExpectedMiningAmount();
    }

    public void UpdateExpectedMiningAmount()
    {
        if (mineGroups == null || amountRoot == null || itemAmountSlotPrefab == null) return;

        foreach (Transform child in amountRoot)
            GameObject.Destroy(child.gameObject);

        var totalResourceDict = new Dictionary<string, float>();
        foreach (var group in mineGroups)
        {
            var resourceDict = CalculateMiningAmount(group, false, out _);
            foreach (var pair in resourceDict)
            {
                if (!totalResourceDict.ContainsKey(pair.Key))
                    totalResourceDict[pair.Key] = 0f;
                totalResourceDict[pair.Key] += pair.Value;
            }
        }

        foreach (var pair in totalResourceDict)
        {
            int amountInt = Mathf.FloorToInt(pair.Value);
            if (amountInt > 0)
            {
                var go = GameObject.Instantiate(itemAmountSlotPrefab, amountRoot);
                var iconImg = go.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
                var amountText = go.transform.Find("Amount").GetComponent<TMPro.TMP_Text>();
                var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);

                // 숫자 세팅
                if (amountText != null)
                    amountText.text = amountInt.ToString();

                // 아이콘 세팅
                if (iconImg != null && item != null)
                {
                    Sprite iconSprite = null;
                    if (!string.IsNullOrEmpty(item.IconPath))
                        iconSprite = IconLoader.GetIconByPath(item.IconPath);
                    if (iconSprite == null && !string.IsNullOrEmpty(item.ItemKey))
                        iconSprite = IconLoader.GetIconByKey(item.ItemKey);
                    if (iconSprite == null)
                        iconSprite = IconLoader.GetIcon(item.ItemType, item.ItemKey);
                    if (iconSprite == null)
                        iconSprite = IconLoader.GetIconByPath("Icons/Empty");

                    iconImg.sprite = iconSprite;
                    iconImg.color = Color.white;
                }
            }
        }
    }

    // 전체 마인 예상 채굴 자원 합산
    private string GetAllExpectedResourcesText()
    {
        var totalResourceDict = new Dictionary<string, float>();
        foreach (var group in mineGroups)
        {
            var resourceDict = CalculateMiningAmount(group, false, out _);
            foreach (var pair in resourceDict)
            {
                if (!totalResourceDict.ContainsKey(pair.Key))
                    totalResourceDict[pair.Key] = 0f;
                totalResourceDict[pair.Key] += pair.Value;
            }
        }

        var sb = new System.Text.StringBuilder();
        foreach (var pair in totalResourceDict)
        {
            int amountInt = Mathf.FloorToInt(pair.Value);
            if (amountInt > 0)
            {
                ItemData item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);
                sb.AppendLine(item != null ? $"{item.Name} x {amountInt}" : $"{pair.Key} x {amountInt}");
            }
        }
        return sb.ToString();
    }

    // 전체 마인 자원 한 번에 수령
    public void CollectAllResources()
    {
        if (mineGroups == null || mineGroups.Count == 0) return;

        var totalRewardList = new List<(string itemKey, int count)>();
        foreach (var group in mineGroups)
        {
            var resourceDict = CalculateMiningAmount(group, true, out float totalHours);

            // 1초 미만은 수집 불가
            if (totalHours < (1.0f / 3600f)) continue;

            foreach (var pair in resourceDict)
            {
                int amountInt = Mathf.FloorToInt(pair.Value);
                if (amountInt > 0)
                {
                    totalRewardList.Add((pair.Key, amountInt));
                    var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(pair.Key);
                    if (item != null)
                        GameManager.Instance.Inventory.AddItem(item, amountInt);
                }
            }

            group.lastCollectTime = DateTime.Now;
            foreach (var slot in group.slots)
            {
                if (slot.IsAssigned)
                    slot.AssignedTime = group.lastCollectTime;
            }
        }

        if (totalRewardList.Count == 0)
        {
            ShowLackPopup(LackType.Resource);
            return;
        }

        if (rewardPopupPrefab && lackpopupRoot)
        {
            var popupObj = Instantiate(rewardPopupPrefab, lackpopupRoot);
            var popup = popupObj.GetComponent<RewardPopup>();
            popup.ShowWithoutManager(
                totalRewardList,
                GameManager.Instance.DataManager.ItemLoader,
                "채광 보상"
            );
        }
        UpdateExpectedMiningAmount();
    }

    private void ShowLackPopup(string msg)
    {
        if (lackPopupPrefab && lackpopupRoot)
        {
            var lackPopup = Instantiate(lackPopupPrefab, lackpopupRoot);
            lackPopup.Init(GameManager.Instance, null);
            lackPopup.ShowCustom(msg);
        }
    }

    private void ShowLackPopup(LackType type)
    {
        if (lackPopupPrefab && lackpopupRoot)
        {
            var lackPopup = Instantiate(lackPopupPrefab, lackpopupRoot);
            lackPopup.Init(GameManager.Instance, null);
            lackPopup.Show(type);
        }
    }

    // 기존 단일 마인 채굴 계산(외부에도 필요)
    public Dictionary<string, float> CalculateMiningAmount(MineGroup group, bool useRandom, out float totalHours)
    {
        totalHours = 0f;
        var resourceDict = new Dictionary<string, float>();
        if (group == null || group.mineManager == null || group.mineManager.Mine == null)
            return resourceDict;

        MineData mineData = group.mineManager.Mine;
        var resourceTypes = mineData.RewardMineralKeys ?? new List<string>();
        foreach (string resKey in resourceTypes)
            resourceDict[resKey] = 0f;

        DateTime now = DateTime.Now;
        totalHours = (float)(now - group.lastCollectTime).TotalHours;
        if (totalHours <= 0) return resourceDict;

        foreach (var slot in group.slots)
        {
            if (!slot.IsAssigned || slot.AssignedAssistant == null)
                continue;

            AssistantInstance assistant = slot.AssignedAssistant;

            float gradeMultiplier = GetGradeMultiplier(assistant.grade);
            float specMultiplier = GetMineSpecMultiplier(assistant);
            float personalityMultiplier = GetPersonalityMiningMultiplier(assistant);
            float totalMultiplier = gradeMultiplier * specMultiplier * personalityMultiplier;

            float elapsedHour = Mathf.Min(totalHours, (float)(now - slot.AssignedTime).TotalHours);
            if (elapsedHour <= 0) continue;

            foreach (string resKey in resourceTypes)
            {
                float randomAmountFactor = useRandom
                    ? UnityEngine.Random.Range(mineData.CollectMin, mineData.CollectMax + 1)
                    : (mineData.CollectMin + mineData.CollectMax) / 2.0f;

                float collectedAmount = mineData.CollectRatePerHour
                                        * randomAmountFactor
                                        * elapsedHour
                                        * totalMultiplier;

                resourceDict[resKey] += collectedAmount;
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
            default: return 1.0f;
        }
    }

    private float GetMineSpecMultiplier(AssistantInstance assistant)
    {
        if (assistant == null || assistant.Multipliers == null) return 1.0f;
        foreach (var m in assistant.Multipliers)
        {
            if (!string.IsNullOrEmpty(m.AbilityName) && m.AbilityName.ToLower().Contains("mine"))
                return m.Multiplier;
        }
        return 1.0f;
    }

    private float GetPersonalityMiningMultiplier(AssistantInstance assistant)
    {
        if (assistant?.Personality == null) return 1.0f;
        return assistant.Personality.miningMultiplier;
    }

    public void SetAmountRoot(Transform root, GameObject prefab)
    {
        amountRoot = root;
        itemAmountSlotPrefab = prefab;
        UpdateExpectedMiningAmount();
    }



}