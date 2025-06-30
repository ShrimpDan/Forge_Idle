using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string ItemKey;
    public int Quantity;

    public int CurrentEnhanceLevel;
    public bool IsEquipped;

    public ItemInstance(string key, ItemData data)
    {
        ItemKey = key;
        Data = data;
        Quantity = 1;
        CurrentEnhanceLevel = 0;
        IsEquipped = false;
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

    public bool EnhanceItem()
    {
        if (CurrentEnhanceLevel >= Data.UpgradeInfo.MaxEnhanceLevel)
        {
            Debug.Log("최대강화치");
            return false;
        }

        CurrentEnhanceLevel++;
        return true;
    }

    public float GetTotalAttack()
    {
        if (Data.ItemType != ItemType.Weapon)
            return 0;

        float multiplier = (CurrentEnhanceLevel + 1) * Data.UpgradeInfo.AttackMultiplier;
        return Data.WeaponStats.Attack * multiplier;
    }

    public float GetTotalInterval()
    {
        if (Data.ItemType != ItemType.Weapon)
            return 0;

        float reduction = (CurrentEnhanceLevel + 1) * Data.UpgradeInfo.IntervalReductionPerLevel;
        return Mathf.Max(0.1f, Data.WeaponStats.AttackInterval - reduction);
    }
}
