using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ForgeInventorySlot : MonoBehaviour
{
    private UIManager uIManager;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    private Button slotBtn;

    private ItemInstance item;
    private Action<ItemInstance> onSlotClick;

    public void Init(ItemInstance item, Action<ItemInstance> onClick)
    {
        this.item = item;
        this.onSlotClick = onClick;

        // ��Ʈ�� Button ���� ����
        slotBtn = GetComponent<Button>();
        if (slotBtn == null)
            slotBtn = gameObject.AddComponent<Button>();

        // ������ ����
        icon.sprite = IconLoader.GetIcon(item.Data.IconPath);
        icon.enabled = (icon.sprite != null);

        // ī��Ʈ ����
        countText.text = item.Quantity.ToString();

        // UIManager ����
        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;

        // Ŭ�� �̺�Ʈ
        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        // �ݹ��� ������ �ѱ��, �ƴϸ� �⺻ �˾� ó��
        if (onSlotClick != null)
            onSlotClick(item);
        else if (uIManager != null && item != null)
        {
            var popup = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
            popup.SetItemInfo(item);
        }
    }
}
