using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string ItemKey;
    public int Quantity;

    public int EnhanceLevel;
    public bool IsEquipped;

    public ItemInstance()
    {
        Quantity = 1;
        EnhanceLevel = 0;
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
        if (EnhanceLevel >= Data.WeaponStats.EnhanceMax)
        {
            Debug.Log("최대강화치");
            return false;
        }

        EnhanceLevel++;
        return true;
    }

    public float GetTotalAttack()
    {
        float multiplier = (EnhanceLevel + 1) * 1.5f;

        if (Data.ItemType != ItemType.Weapon)
            return 0;

        return Data.WeaponStats.Attack * multiplier;
    }

    public float GetTotalInterval()
    {
        if (Data.ItemType != ItemType.Weapon)
            return 0;

        return Mathf.Max(0.1f, Data.WeaponStats.AttackInterval - (EnhanceLevel + 1) * 0.2f);
    }
}
