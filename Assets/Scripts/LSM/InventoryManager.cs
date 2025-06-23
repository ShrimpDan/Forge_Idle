using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<Item> items = new List<Item>();
    public int gold = 1000;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public int CountItem(Item item)
    {
        int count = 0;
        foreach (var i in items)
        {
            if (i == item)
                count++;
        }
        return count;
    }

    public bool HasEnoughItems(Item item, int count)
    {
        return CountItem(item) >= count;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public void EarnGold(int amount)
    {
        gold += amount;
    }
}
