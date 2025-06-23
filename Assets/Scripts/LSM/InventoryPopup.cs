using UnityEngine;
using System.Collections.Generic;

public class InventoryPopup : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentParent;

    System.Action<Item> onSelect;

    public void Show(List<Item> items, System.Action<Item> callback)
    {
        onSelect = callback;
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            var slot = Instantiate(slotPrefab, contentParent).GetComponent<ItemSlot>();
            slot.Set(item, SelectItem);
        }
        gameObject.SetActive(true);
    }

    void SelectItem(Item item)
    {
        gameObject.SetActive(false);
        onSelect?.Invoke(item);
    }
}
