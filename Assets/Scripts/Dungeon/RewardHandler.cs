using System.Collections.Generic;
using UnityEngine;

public class RewardHandler : MonoBehaviour
{
    private DungeonUI dungeonUI;
    private Jang.InventoryManager inventory;

    public Dictionary<ItemData, int> RewardItems { get; private set; }

    public void Init(Jang.InventoryManager inventory, DungeonUI dungeonUI)
    {
        this.inventory = inventory;
        this.dungeonUI = dungeonUI;

        RewardItems = new();
    }

    public void AddReward(ItemData item, int amount)
    {
        if (RewardItems.ContainsKey(item))
        {
            RewardItems[item] += amount;
            dungeonUI.UpdateRewardInfo(item, amount);
            return;
        }

        RewardItems[item] = amount;
        dungeonUI.UpdateRewardInfo(item, amount);
    }

    public void ApplyReward()
    {
        foreach (var item in RewardItems.Keys)
        {
            inventory.AddItem(item, RewardItems[item]);
        }
    }
}
