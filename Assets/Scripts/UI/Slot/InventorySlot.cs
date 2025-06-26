using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private UIManager uIManager;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    private Button slotBtn;

    public ItemType ItemType { get; private set; }
    public ItemInstance ItemInstance { get; private set; }

    public void Init(ItemInstance itemInstance)
    {
        slotBtn = GetComponent<Button>();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(() => OnClickSlot());

        ItemInstance = itemInstance;

        icon.sprite = Resources.Load<Sprite>(ItemInstance.Data.IconPath);
        countText.text = itemInstance.Quantity.ToString();

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;
    }

    private void OnClickSlot()
    {
        if (ItemInstance == null) return;

        var ui = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        ui.SetItemInfo(ItemInstance);
    }
}
