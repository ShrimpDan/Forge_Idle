using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ForgeInventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button slotBtn;

    private ItemInstance item;

    public void Init(ItemInstance item, Action<ItemInstance> onClick)
    {
        this.item = item;

        // ������
        if (icon != null && item?.Data != null)
        {
            Sprite loaded = IconLoader.GetIcon(item.Data.IconPath);
            icon.sprite = loaded;
            icon.enabled = loaded != null;
        }
        else if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        // ī��Ʈ
        if (countText != null)
            countText.text = item != null ? item.Quantity.ToString() : "";

        // Ŭ�� �̺�Ʈ
        if (slotBtn != null)
        {
            slotBtn.onClick.RemoveAllListeners();
            if (onClick != null)
                slotBtn.onClick.AddListener(() => onClick(this.item));
        }
    }
}
