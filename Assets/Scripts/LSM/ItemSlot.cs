using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    Item item;
    System.Action<Item> onClick;

    public void Set(Item item, System.Action<Item> onClick)
    {
        this.item = item;
        this.onClick = onClick;
        icon.sprite = item.icon;
        nameText.text = item.itemName;
        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        onClick?.Invoke(item);
    }
}
