using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MineResourceCollectManager : MonoBehaviour
{
    public static MineResourceCollectManager Instance;

    [Header("UI Roots & Prefabs")]
    [SerializeField] private Transform amountRoot;
    [SerializeField] private GameObject itemAmountSlotPrefab;

    [Header("빠른 수령 버튼!")]
    [SerializeField] private Button quickCollectButton;

    private List<MineGroup> mineGroups;
    private int currentMineGroupIndex = 0;

    [SerializeField] private float updateInterval = 1.0f;
    private float timer = 0f;

    [Header("UI Prefabs ")]
    [SerializeField] private GameObject rewardPopupPrefab;
    [SerializeField] private LackPopup lackPopupPrefab;
    [SerializeField] private Transform lackpopupRoot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        // 빠른수령 버튼 리스너 등록
        if (quickCollectButton != null)
        {
            quickCollectButton.onClick.RemoveAllListeners();
            quickCollectButton.onClick.AddListener(OnQuickCollectClicked);
        }
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

                if (amountText != null)
                    amountText.text = amountInt.ToString();

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

    //퀵 콜렉트 - 3시간치 자원
    private void OnQuickCollectClicked()
    {
        var forgeManager = GameManager.Instance?.ForgeManager;
        // 다이아 100 사용. 실패 시 부족팝업
        if (!forgeManager.UseDia(100))
        {
            ShowLackPopup(LackType.Dia);
            return;
        }

        // 3시간치 자원 지급
        float quickHours = 3.0f;
        var totalRewardList = new List<(string itemKey, int count)>();

        foreach (var group in mineGroups)
        {
            if (group == null || group.mineManager == null || group.mineManager.Mine == null) continue;

            MineData mineData = group.mineManager.Mine;
            var resourceTypes = mineData.RewardMineralKeys ?? new List<string>();
            foreach (string resKey in resourceTypes)
            {
                float totalCollected = 0f;
                foreach (var slot in group.slots)
                {
                    if (!slot.IsAssigned || slot.AssignedAssistant == null)
                        continue;

                    AssistantInstance assistant = slot.AssignedAssistant;
                    float gradeMultiplier = GetGradeMultiplier(assistant.grade);
                    float specMultiplier = GetMineSpecMultiplier(assistant);
                    float personalityMultiplier = GetPersonalityMiningMultiplier(assistant);
                    float totalMultiplier = gradeMultiplier * specMultiplier * personalityMultiplier;

                    // 3시간 기준으로 보상 계산 (랜덤없이 평균)
                    float randomAmountFactor = (mineData.CollectMin + mineData.CollectMax) / 2.0f;
                    float collectedAmount = mineData.CollectRatePerHour
                                            * randomAmountFactor
                                            * quickHours
                                            * totalMultiplier;

                    totalCollected += collectedAmount;
                }

                int intAmount = Mathf.FloorToInt(totalCollected);
                if (intAmount > 0)
                {
                    totalRewardList.Add((resKey, intAmount));
                    var item = GameManager.Instance.DataManager.ItemLoader.GetItemByKey(resKey);
                    if (item != null)
                        GameManager.Instance.Inventory.AddItem(item, intAmount);
                }
            }
        }

        if (totalRewardList.Count == 0)
        {
            ShowLackPopup("수령할 자원이 부족합니다.");
            return;
        }

        // 보상 팝업 노출
        if (rewardPopupPrefab && lackpopupRoot)
        {
            var popupObj = Instantiate(rewardPopupPrefab, lackpopupRoot);
            var popup = popupObj.GetComponent<RewardPopup>();
            popup.ShowWithoutManager(
                totalRewardList,
                GameManager.Instance.DataManager.ItemLoader,
                "빠른 수령(3시간)"
            );
        }
        UpdateExpectedMiningAmount();
    }

    // 기존 1회 전체 수령 함수(일반 버튼용)
    public void CollectAllResources()
    {
        if (mineGroups == null || mineGroups.Count == 0) return;

        var totalRewardList = new List<(string itemKey, int count)>();
        foreach (var group in mineGroups)
        {
            var resourceDict = CalculateMiningAmount(group, true, out float totalHours);

            // 1초 미만은 지급 없음
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
                "획득 자원"
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

    // MineGroup 자원 계산 함수(기존)
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
        totalHours =  Mathf.Min((float)(now - group.lastCollectTime).TotalHours, 12);
        if (totalHours <= 0) return resourceDict;

        // --- SceneManager 가져오기 (버프 연동) ---
        var sceneManager = FindObjectOfType<MineSceneManager>();

        foreach (var slot in group.slots)
        {
            if (!slot.IsAssigned || slot.AssignedAssistant == null)
                continue;

            AssistantInstance assistant = slot.AssignedAssistant;

            float gradeMultiplier = GetGradeMultiplier(assistant.grade);
            float specMultiplier = GetMineSpecMultiplier(assistant);
            float personalityMultiplier = GetPersonalityMiningMultiplier(assistant);
            float fsmBuffMultiplier = 1.0f;

            if (sceneManager != null)
                fsmBuffMultiplier = sceneManager.GetBuffMultiplierForSlot(slot);

            float totalMultiplier = gradeMultiplier * specMultiplier * personalityMultiplier * fsmBuffMultiplier;

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
