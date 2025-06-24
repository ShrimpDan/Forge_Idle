using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<Item> items = new List<Item>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public List<Item> GetAllWeapons()
    {
        return items.FindAll(i => i is Weapon);
    }

    public List<Item> GetAllGems()
    {
        return items.FindAll(i => i is Gem);
    }
}
