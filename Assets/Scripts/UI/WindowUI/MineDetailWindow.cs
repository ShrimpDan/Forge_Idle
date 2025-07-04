using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class MineDetailWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button collectAllBtn;
    [SerializeField] private Transform mineralSlotParent;
    [SerializeField] private GameObject mineralSlotPrefab;

    private List<MineralSlot> mineralSlots = new();
    private MineData mineData;
    private DataManager dataManager;
    private GameManager gameManager;
    private UIManager uIManager;

    // 드랍테이블: 마인별 자원, 확률(weight)
    private static readonly Dictionary<string, List<(string key, float weight)>> mineDropTable = new()
    {
        ["mine_bronze"] = new List<(string, float)> {
            ("resource_bronze", 80f),
            ("resource_copper", 15f),
            ("resource_iron", 4f),
            ("resource_silver", 0.8f),
            ("resource_gold", 0.2f)
        },
        ["mine_silver"] = new List<(string, float)> {
            ("resource_silver", 75f),
            ("resource_gold", 15f),
            ("resource_iron", 8f),
            ("resource_mithril", 1.5f),
            ("resource_amethyst", 0.5f)
        },
        ["mine_crystal"] = new List<(string, float)> {
            ("resource_iron", 60f),
            ("resource_silver", 20f),
            ("resource_amethyst", 8f),
            ("resource_sapphire", 6f),
            ("resource_gold", 5f),
            ("resource_mithril", 1f)
        },
        ["mine_ruby"] = new List<(string, float)> {
            ("resource_gold", 50f),
            ("resource_silver", 20f),
            ("resource_mithril", 8f),
            ("resource_ruby", 1.5f),
            ("resource_amethyst", 10f),
            ("resource_sapphire", 10f)
        },
        ["mine_emerald"] = new List<(string, float)> {
            ("resource_gold", 50f),
            ("resource_emerald", 2f),
            ("resource_amethyst", 10f),
            ("resource_sapphire", 10f),
            ("resource_mithril", 8f),
            ("resource_ruby", 0.5f)
        }
    };

    private class MineralCollectionInfo
    {
        public TraineeData assignedTrainee;
        public DateTime assignTime;
        public float pendingReward;
        public string mineralKey;
    }
    private List<MineralCollectionInfo> collectionInfos = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineDetailWindow));
    }

    public void SetupMine(MineData mineData, DataManager dataManager, GameManager gameManager, UIManager uIManager)
    {
        this.mineData = mineData;
        this.dataManager = dataManager;
        this.gameManager = gameManager;
        this.uIManager = uIManager;

        foreach (Transform child in mineralSlotParent)
            Destroy(child.gameObject);
        mineralSlots.Clear();
        collectionInfos.Clear();

        var itemLoader = dataManager.ItemLoader;

        for (int i = 0; i < 5; i++)
        {
            string key = PickDropResourceKey(mineData.Key);
            ItemData resourceData = itemLoader?.GetItemByKey(key);
            Sprite icon = resourceData != null ? IconLoader.GetIcon(resourceData.IconPath) : null;
            string mineralName = resourceData != null ? resourceData.Name : $"광물{i + 1}";
            GameObject go = Instantiate(mineralSlotPrefab, mineralSlotParent);
            int idx = i;
            var info = new MineralCollectionInfo { assignedTrainee = null, assignTime = DateTime.MinValue, pendingReward = 0, mineralKey = key };
            collectionInfos.Add(info);

            MineralSlot slot = go.GetComponent<MineralSlot>();
            slot.Init(mineralName, icon, () => OnClickAssignAssistant(idx));
            mineralSlots.Add(slot);
        }

        if (collectAllBtn != null)
        {
            collectAllBtn.onClick.RemoveAllListeners();
            collectAllBtn.onClick.AddListener(CollectAllReward);
        }
    }

    private string PickDropResourceKey(string mineKey)
    {
        if (!mineDropTable.ContainsKey(mineKey))
            return "";

        var dropList = mineDropTable[mineKey];
        float totalWeight = 0;
        foreach (var pair in dropList)
            totalWeight += pair.weight;

        float r = UnityEngine.Random.Range(0, totalWeight);
        float sum = 0;
        foreach (var pair in dropList)
        {
            sum += pair.weight;
            if (r <= sum)
                return pair.key;
        }
        return dropList[0].key;
    }

    private void OnClickAssignAssistant(int idx)
    {
        var info = collectionInfos[idx];
        var traineeInventory = gameManager.TraineeInventory;

        if (info.assignedTrainee != null)
        {
            traineeInventory.Add(info.assignedTrainee);
            info.assignedTrainee = null;
            info.assignTime = DateTime.MinValue;
            info.pendingReward = 0;
            mineralSlots[idx].SetAssistant(null);
            Debug.Log("어시스턴트 해제됨, 채집 중단");
            return;
        }

        // AssistantSelectPopup 사용
        var popup = uIManager.OpenUI<AssistantSelectPopup>(UIName.AssistantSelectPopup);
        popup.Init(gameManager, uIManager);

        popup.OpenForSelection((trainee) =>
        {
            if (trainee == null) return;
            // Mine_AssistantPopup 호출
            var assiPopup = uIManager.OpenUI<Mine_AssistantPopup>(UIName.Mine_AssistantPopup);
            assiPopup.Init(gameManager, uIManager);

            assiPopup.SetAssistant(trainee, false, (selected, isAssign) =>
            {
                if (isAssign)
                {
                    info.assignedTrainee = selected;
                    info.assignTime = DateTime.Now;
                    info.pendingReward = 0;
                    traineeInventory.Remove(selected);
                    mineralSlots[idx].SetAssistant(selected);
                    Debug.Log($"어시스턴트 배정, 채집 시작: {selected.Name}");
                }
                else
                {
                    if (info.assignedTrainee != null)
                        traineeInventory.Add(info.assignedTrainee);

                    info.assignedTrainee = null;
                    info.assignTime = DateTime.MinValue;
                    info.pendingReward = 0;
                    mineralSlots[idx].SetAssistant(null);
                    Debug.Log("어시스턴트 해제됨, 채집 중단");
                }
            });
        });
    }

    private float CalcCollectedAmount(MineralCollectionInfo info)
    {
        if (info.assignedTrainee == null) return 0;
        double elapsedSec = (DateTime.Now - info.assignTime).TotalSeconds;
        float hour = (float)(elapsedSec / 3600f);
        int randQty = UnityEngine.Random.Range(mineData.CollectMin, mineData.CollectMax + 1);
        float amount = hour * mineData.CollectRatePerHour * randQty;
        return amount;
    }

    private void CollectAllReward()
    {
        var inventory = gameManager.Inventory;
        var itemLoader = dataManager.ItemLoader;
        for (int i = 0; i < collectionInfos.Count; i++)
        {
            var info = collectionInfos[i];
            if (info.assignedTrainee == null) continue;
            float amount = CalcCollectedAmount(info);
            info.pendingReward += amount;
            int rewardInt = Mathf.FloorToInt(info.pendingReward);
            if (rewardInt > 0)
            {
                ItemData mineral = itemLoader.GetItemByKey(info.mineralKey);
                if (mineral != null)
                {
                    inventory.AddItem(mineral, rewardInt);
                    Debug.Log($"광물 {mineral.Name} x{rewardInt} 지급!");
                }
                info.pendingReward -= rewardInt;
                info.assignTime = DateTime.Now;
            }
        }
    }
}
