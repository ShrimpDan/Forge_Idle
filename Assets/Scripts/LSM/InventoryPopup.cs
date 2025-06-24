using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    // [SerializeField] 등으로 슬롯/리스트 필드 선언

    private Action<Item> onSelectCallback;

    public void Show(List<Item> items, Action<Item> onSelect)
    {
        // 슬롯 리스트 UI 갱신
        onSelectCallback = onSelect;
        // 각 슬롯에 버튼 연결 시: => OnItemClick(item)
    }

    public void OnItemClick(Item item)
    {
        onSelectCallback?.Invoke(item);
        UIManager.Instance.CloseUI(UIName.InventoryPopup);
    }

    public override void Open() => gameObject.SetActive(true);
    public override void Close() => gameObject.SetActive(false);
}
