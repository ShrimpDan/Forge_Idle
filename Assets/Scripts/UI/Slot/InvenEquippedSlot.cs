using UnityEngine;
using UnityEngine.UI;

public class InvenEquippedSlot : MonoBehaviour
{
    private UIManager uIManager;

    public ItemInstance EquippedItem { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;

    public void Init(UIManager uIManager, ItemInstance item)
    {
        if (item == null)
        {
            UnEquipItem();
            return;
        }

        if (this.uIManager == null)
            this.uIManager = uIManager;

        EquippedItem = item;
        icon.sprite = IconLoader.GetIcon(item.Data.IconPath);

        slotBtn = GetComponent<Button>();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        if (EquippedItem == null) return;

        var ui = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        ui.SetItemInfo(EquippedItem);
    }

    public void UnEquipItem()
    {
        EquippedItem = null;
        icon.sprite = null;
    }
}

