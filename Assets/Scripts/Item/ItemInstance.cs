using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string ItemKey;
    public int Quantity;

    public int CurrentEnhanceLevel;
    public bool IsEquipped;
    public bool CanEnhance => CurrentEnhanceLevel < Data.UpgradeInfo.MaxEnhanceLevel;

    public CraftingData CraftingData { get; private set; }

    public ItemInstance(string key, ItemData data, CraftingData craftingData = null)
    {
        ItemKey = key;
        Data = data;
        Quantity = 1;
        CurrentEnhanceLevel = 0;
        IsEquipped = false;
        CraftingData = craftingData;
    }

    [System.NonSerialized]
    public ItemData Data;

    public void EquipItem()
    {
        Debug.Log("Equip Item");
        IsEquipped = true;
    }

    public void UnEquipItem()
    {
        Debug.Log("UnEquip Item");
        IsEquipped = false;
    }

    public void AddItem(int count)
    {
        Quantity += count;
    }

    public void UseItem()
    {
        Debug.Log("Use Item");
        Quantity -= 1;
    }

    public void EnhanceItem()
    {
        if(CanEnhance)
            CurrentEnhanceLevel++;
    }

    public float GetTotalAttack()
    {
        if (Data.ItemType != ItemType.Weapon)
            return 0;

        if (CurrentEnhanceLevel == 0)
            return Data.WeaponStats.Attack;

        float multiplier = CurrentEnhanceLevel * Data.UpgradeInfo.AttackMultiplier;
        return Data.WeaponStats.Attack * multiplier;
    }

    public float GetTotalInterval()
    {
        if (Data.ItemType != ItemType.Weapon)
            return 0;

        if (CurrentEnhanceLevel == 0)
            return Data.WeaponStats.AttackInterval;

        float reduction = CurrentEnhanceLevel * Data.UpgradeInfo.IntervalReductionPerLevel;
        return Mathf.Max(0.1f, Data.WeaponStats.AttackInterval - reduction);
    }
}
