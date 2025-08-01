using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private UIManager uIManager;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject equippedIndicator;
    private Button slotBtn;

    public ItemType ItemType { get; private set; }
    public ItemInstance SlotItem { get; private set; }

    public void Init(ItemInstance item)
    {
        slotBtn = GetComponent<Button>();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(() => OnClickSlot());

        SlotItem = item;
        countText.text = item.Data.Name;
        icon.sprite = IconLoader.GetIconByKey(SlotItem.ItemKey);

        if (uIManager == null)
                uIManager = GameManager.Instance.UIManager;

        SetEquipped(SlotItem.IsEquipped);
    }

    private void OnClickSlot()
    {
        if (SlotItem == null) return;

        SoundManager.Instance.Play("SFX_SystemClick");

        var ui = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        ui.SetItemInfo(SlotItem);
    }

    public void SetEquipped(bool isEquipped)
    {
        equippedIndicator.SetActive(isEquipped);
    }
}
