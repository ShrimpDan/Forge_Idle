using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    // [SerializeField] ������ ����/����Ʈ �ʵ� ����

    private Action<Item> onSelectCallback;

    public void Show(List<Item> items, Action<Item> onSelect)
    {
        // ���� ����Ʈ UI ����
        onSelectCallback = onSelect;
        // �� ���Կ� ��ư ���� ��: => OnItemClick(item)
    }

    public void OnItemClick(Item item)
    {
        onSelectCallback?.Invoke(item);
        UIManager.Instance.CloseUI(UIName.InventoryPopup);
    }

    public override void Open() => gameObject.SetActive(true);
    public override void Close() => gameObject.SetActive(false);
}
