using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<ItemInstance> ResourceList { get; private set; }
    public List<ItemInstance> EquipmentList { get; private set; }
    public List<ItemInstance> GemList { get; private set; }

    public Inventory()
    {
        ResourceList = new();
        EquipmentList = new();
        GemList = new();
    }

    public void AddItem(ItemInstance item)
    {
        switch (item.Data.ItemType)
        {
            case ItemType.Equipment:
                EquipmentList.Add(item);
                break;

            case ItemType.Gem:
                GemList.Add(item);
                break;

            case ItemType.Resource:
                ResourceList.Add(item);
                break;
        }
    }
}
