using UnityEngine;
using UnityEngine.UI;
using System;

public class Forge_ItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;   // 자식 오브젝트의 Icon(Image)과 연결
    private Button btn;
    private ItemInstance item;
    private Action<ItemInstance> onClick;

    public void Init(ItemInstance item, Action<ItemInstance> onClick)
    {
        this.item = item;
        this.onClick = onClick;

        btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();

        if (icon != null && item?.Data != null)
        {
            icon.sprite = IconLoader.GetIcon(item.Data.IconPath);
            icon.enabled = icon.sprite != null;
        }

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => this.onClick?.Invoke(item));
    }
}
