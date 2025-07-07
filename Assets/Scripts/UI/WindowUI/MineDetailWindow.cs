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

    private class MineralCollectionInfo
    {
        public AssistantData assignedAssistant;
        public DateTime assignTime;
        public float pendingReward;
        public string mineralKey;
    }
    private List<MineralCollectionInfo> collectionInfos = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;
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
            string key = "resource_bronze"; // 예시용. 드랍키 로직 추가 필요
            ItemData resourceData = itemLoader?.GetItemByKey(key);
            Sprite icon = resourceData != null ? IconLoader.GetIcon(resourceData.IconPath) : null;
            string mineralName = resourceData != null ? resourceData.Name : $"광물{i + 1}";
            GameObject go = Instantiate(mineralSlotPrefab, mineralSlotParent);
            int idx = i;
            var info = new MineralCollectionInfo { assignedAssistant = null, assignTime = DateTime.MinValue, pendingReward = 0, mineralKey = key };
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

    private void OnClickAssignAssistant(int idx)
    {
        var info = collectionInfos[idx];
        var assistantInventory = gameManager.AssistantManager.AssistantInventory;

        if (info.assignedAssistant != null)
        {
            assistantInventory.Add(info.assignedAssistant);
            info.assignedAssistant = null;
            info.assignTime = DateTime.MinValue;
            info.pendingReward = 0;
            mineralSlots[idx].SetAssistant(null);
            return;
        }

        var popup = uIManager.OpenUI<AssistantSelectPopup>(UIName.AssistantSelectPopup);
        popup.Init(gameManager, uIManager);

        popup.OpenForSelection((assistant) =>
        {
            if (assistant == null) return;
            var assiPopup = uIManager.OpenUI<Mine_AssistantPopup>(UIName.Mine_AssistantPopup);
            assiPopup.Init(gameManager, uIManager);

            assiPopup.SetAssistant(assistant, false, (selected, isAssign) =>
            {
                if (isAssign)
                {
                    info.assignedAssistant = selected;
                    info.assignTime = DateTime.Now;
                    info.pendingReward = 0;
                    assistantInventory.Remove(selected);
                    mineralSlots[idx].SetAssistant(selected);
                }
                else
                {
                    if (info.assignedAssistant != null)
                        assistantInventory.Add(info.assignedAssistant);

                    info.assignedAssistant = null;
                    info.assignTime = DateTime.MinValue;
                    info.pendingReward = 0;
                    mineralSlots[idx].SetAssistant(null);
                }
                uIManager.CloseUI(UIName.Mine_AssistantPopup);
            });
        });
    }
}
