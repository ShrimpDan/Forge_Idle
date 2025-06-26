using UnityEngine;
using UnityEngine.UI;

public class InvenEquippedSlot : MonoBehaviour
{
    private UIManager uIManager;

    public ItemInstance equippedItem { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;

    public void Init(ItemInstance item)
    {
        equippedItem = item;
        icon.sprite = IconLoader.GetIcon(item.Data.IconPath);

        if (uIManager == null)
        {
            uIManager = GameManager.Instance.UIManager;
        }

        slotBtn = GetComponent<Button>();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);
    }
    
    private void OnClickSlot()
    {
        if (equippedItem == null) return;

        var ui = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        ui.SetItemInfo(equippedItem);
    }

    public void UnEquipItem()
    {
        equippedItem = null;
        icon.sprite = null;
    }
}

