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

    [Range(0f, 1f)]
    [SerializeField] private float firstRewardProbability = 0.7f;

    private List<MineralSlot> mineralSlots = new();
    private MineData mineData;
    private DataManager dataManager;
    private GameManager gameManager;
    private UIManager uIManager;

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
            string key = PickRandomResourceKey();
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

    private string PickRandomResourceKey()
    {
        if (mineData.RewardMineralKeys == null || mineData.RewardMineralKeys.Count < 2)
            return mineData.RewardMineralKeys?[0] ?? "";
        float r = UnityEngine.Random.value;
        return (r < firstRewardProbability) ? mineData.RewardMineralKeys[0] : mineData.RewardMineralKeys[1];
    }

    private void OnClickAssignAssistant(int idx)
    {
        var info = collectionInfos[idx];
        var traineeInventory = gameManager.TraineeInventory;
        // 어시스턴트 이미 배정? -> 해제
        if (info.assignedTrainee != null)
        {
            traineeInventory.Add(info.assignedTrainee);
            info.assignedTrainee = null;
            info.assignTime = DateTime.MinValue;
            info.pendingReward = 0;
            Debug.Log("어시스턴트 해제됨, 채집 중단");
            return;
        }
        // Trainee 선택 콜백
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetTraineeSelectCallback((trainee) =>
        {
            if (trainee == null) return;
            info.assignedTrainee = trainee;
            info.assignTime = DateTime.Now;
            info.pendingReward = 0;
            traineeInventory.Remove(trainee);
            Debug.Log($"어시스턴트 배정, 채집 시작: {trainee.Name}");
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
