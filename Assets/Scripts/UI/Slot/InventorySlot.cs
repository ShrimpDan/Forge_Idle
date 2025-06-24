using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour
{
    [Header("Test용")]
    [SerializeField] private string itemKey;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    private Button slotBtn;

    public ItemType ItemType { get; private set; }
    public ItemInstance ItemInstance { get; private set; }

    void Awake()
    {
        slotBtn = GetComponent<Button>();
        slotBtn.onClick.AddListener(() => OnClickSlot());
    }

    void Start()
    {
        ItemInstance = new ItemInstance();
        ItemInstance.Data = TestDataManger.Instance.ItemLoader.GetItemByKey(itemKey);
    }

    public void Init(ItemInstance itemInstance)
    {
        ItemInstance = itemInstance;
    }

    private void OnClickSlot()
    {
        if (ItemInstance == null) return;
        ItemData data = ItemInstance.Data;

        var ui = UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);

        switch (ItemInstance.Data.ItemType)
        {
            case ItemType.Equipment:
                OpenEquipmentPopup(ui);
                break;

            case ItemType.Gem:
                OpenGemPopup(ui);
                break;

            case ItemType.Resource:
                OpenResourcePopup(ui);
                break;
        }
    }

    private void OpenEquipmentPopup(InventoryPopup popup)
    {
        ItemData data = ItemInstance.Data;

        popup.SetContext(
            new InventoryContext
            {
                Icon = LoadIcon(data.IconPath),
                Name = data.Name,
                Count = ItemInstance.Quantity,
                Description = data.Description,

                Actions = new List<(string label, UnityEngine.Events.UnityAction action)>
                {
                    (label: "Equip", action: () => ItemInstance.EquipItem())
                }
            }
        );
    }

    private void OpenGemPopup(InventoryPopup popup)
    {
        ItemData data = ItemInstance.Data;

        popup.SetContext(
            new InventoryContext
            {
                Icon = LoadIcon(data.IconPath),
                Name = data.Name,
                Count = ItemInstance.Quantity,
                Description = data.Description,
            }
        );
    }

    private void OpenResourcePopup(InventoryPopup popup)
    {
        ItemData data = ItemInstance.Data;

        popup.SetContext(
            new InventoryContext
            {
                Icon = LoadIcon(data.IconPath),
                Name = data.Name,
                Count = ItemInstance.Quantity,
                Description = data.Description,
            }
        );
    }

    private Sprite LoadIcon(string path)
    {
        Sprite icon = Resources.Load<Sprite>(path);

        if (icon != null)
            return icon;

        return null;
    }
}
