using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RefineListSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Button selectButton;

    private ItemData itemData;
    private Action onClick;

    public void Set(ItemData data, Action onClickCallback)
    {
        itemData = data;
        onClick = onClickCallback;
        if (icon != null) icon.sprite = IconLoader.GetIconByPath(data.IconPath);
        if (itemName != null) itemName.text = data.Name;
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
