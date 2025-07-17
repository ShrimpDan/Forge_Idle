using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeUpgradeWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    private Forge forge;
    private ForgeUpgradeDataLoader upgradeDataLoader;
    [SerializeField] private Button exitBtn;
    [SerializeField] private List<ForgeUpgradeSlot> upgradeSlotList = new List<ForgeUpgradeSlot>();
    private Dictionary<ForgeUpgradeType, ForgeUpgradeSlot> upgradeSlotDict = new Dictionary<ForgeUpgradeType, ForgeUpgradeSlot>();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        forge = gameManager.ForgeManager.CurrentForge;
        upgradeDataLoader = gameManager.DataManager.UpgradeDataLoader;

        foreach (var slot in upgradeSlotList)
        {
            slot.Init(this);
            upgradeSlotDict[slot.UpgradeType] = slot;
            SetUpgradeSlot(slot);
        }

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.ForgeUpgradeWindow));
    }

    public override void Open()
    {
        base.Open();
        
        foreach (var slot in upgradeSlotList)
        {
            SetUpgradeSlot(slot);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public void UpgradeForge(ForgeUpgradeSlot slot)
    {
        if (forge.StatHandler.TryUpgradeStat(slot.UpgradeType))
        {
            SetUpgradeSlot(slot);
            return;
        }
    }

    private void SetUpgradeSlot(ForgeUpgradeSlot slot)
    {
        int level = forge.StatHandler.UpgradeLevels[slot.UpgradeType];
        int maxLevel = upgradeDataLoader.GetMaxLevel(forge.ForgeType, slot.UpgradeType);
        float curValue = upgradeDataLoader.GetValue(forge.ForgeType, slot.UpgradeType, level);

        if (level < maxLevel)
        {
            float nextValue = upgradeDataLoader.GetValue(forge.ForgeType, slot.UpgradeType, level + 1);
            int cost = upgradeDataLoader.GetCost(forge.ForgeType, slot.UpgradeType, level);

            slot.SetSlot(level, curValue, nextValue, cost);
        }
        else if (level == maxLevel)
        {
            slot.SetSlot(level, curValue);
        }
    }
}
