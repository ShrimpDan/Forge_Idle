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
        EnhanceLevel = 1;
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
}
