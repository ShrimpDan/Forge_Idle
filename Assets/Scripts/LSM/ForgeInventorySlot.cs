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

        // 루트에 Button 강제 참조
        slotBtn = GetComponent<Button>();
        if (slotBtn == null)
            slotBtn = gameObject.AddComponent<Button>();

        // 아이콘 세팅
        icon.sprite = IconLoader.GetIcon(item.Data.IconPath);
        icon.enabled = (icon.sprite != null);

        // 카운트 세팅
        countText.text = item.Quantity.ToString();

        // UIManager 참조
        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;

        // 클릭 이벤트
        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        // 콜백이 있으면 넘기고, 아니면 기본 팝업 처리
        if (onSlotClick != null)
            onSlotClick(item);
        else if (uIManager != null && item != null)
        {
            var popup = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
            popup.SetItemInfo(item);
        }
    }
}
