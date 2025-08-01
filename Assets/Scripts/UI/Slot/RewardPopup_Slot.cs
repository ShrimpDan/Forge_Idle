using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPopup_Slot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private string itemKey;
    private int quantity = 0;

    public void Init(ItemData itemData, int count)
    {
        itemKey = itemData.ItemKey;
        quantity = count;
        quantityText.text = quantity.ToString();

        Sprite iconSprite = null;

        if (!string.IsNullOrEmpty(itemData.IconPath))
            iconSprite = IconLoader.GetIconByPath(itemData.IconPath);

        if (iconSprite == null && !string.IsNullOrEmpty(itemKey))
            iconSprite = IconLoader.GetIconByKey(itemKey);

        if (iconSprite == null)
            iconSprite = IconLoader.GetIcon(itemData.ItemType, itemKey);

        if (iconSprite == null)
            iconSprite = IconLoader.GetIconByPath("Icons/Empty");

        icon.sprite = iconSprite;

        if (icon != null) icon.color = Color.white;
    }

    public void Add(int count)
    {
        quantity += count;
        quantityText.text = quantity.ToString();
    }
}
