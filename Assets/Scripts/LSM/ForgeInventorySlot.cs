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

        // 아이콘
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

        // 카운트
        if (countText != null)
            countText.text = item != null ? item.Quantity.ToString() : "";

        // 클릭 이벤트
        if (slotBtn != null)
        {
            slotBtn.onClick.RemoveAllListeners();
            if (onClick != null)
                slotBtn.onClick.AddListener(() => onClick(this.item));
        }
    }
}
