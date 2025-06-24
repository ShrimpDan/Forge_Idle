using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string Key;
    public InvenSlotType SlotType;
    public string IconPath;
    public string Name;
    public float Value;
    public string Description;

    public void UseItem()
    {
        Debug.Log("아이템 사용");
    }
}

public class ItemDataLoader
{
    public List<ItemData> ItemList { get; private set; }
    public Dictionary<string, ItemData> ItemDict { get; private set; }

    public ItemDataLoader()
    {
        ItemList = new List<ItemData>
        {
            new ItemData
            {
                Key = "testItem1",
                SlotType = InvenSlotType.Resource,
                IconPath = string.Empty,
                Name = "TestItem",
                Value = 51000,
                Description = "I'm Stupid"
            },

            new ItemData
            {
                Key = "testItem2",
                SlotType = InvenSlotType.Useable,
                IconPath = null,
                Name = "testUsable",
                Value = 5000,
                Description = "Recovery 500 Health",
            },
        };

        ItemDict = new Dictionary<string, ItemData>();

        foreach (var item in ItemList)
        {
            ItemDict.Add(item.Key, item);
        }
    }

    public ItemData GetItemByKey(string key)
    {
        if (ItemDict.TryGetValue(key, out ItemData data))
            return data;
        
        return null;
    }
}

