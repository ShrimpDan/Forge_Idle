using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("Test용")]
    [SerializeField] private string itemKey;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    private Button slotBtn;

    public InvenSlotType SlotType { get; private set; }
    public ItemData ItemData { get; private set; }

    private ItemDataLoader itemDataLoader;

    void Awake()
    {
        slotBtn = GetComponent<Button>();
        slotBtn.onClick.AddListener(() => OnClickSlot());

        itemDataLoader = new ItemDataLoader();
    }

    void Start()
    {
        ItemData = itemDataLoader.GetItemByKey(itemKey);
    }

    public void Init(ItemData itemData, InvenSlotType type)
    {
        ItemData = itemData;
        SlotType = type;
        countText.text = itemData.Count.ToString();
    }

    private void OnClickSlot()
    {
        if (ItemData == null) return;

        switch (ItemData.SlotType)
        {
            case InvenSlotType.Resource:
                OpenResourcePopup();
                break;

            case InvenSlotType.Useable:
                OpenUsablePopup();
                break;
        }
    }

    private void OpenResourcePopup()
    {
        var popup = UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        popup.SetContext(
            new InventoryContext
            {
                Icon = LoadIcon(ItemData.IconPath),
                Name = ItemData.Name,
                Count = ItemData.Count,
                Description = ItemData.Description,
            }
        );
    }

    private void OpenUsablePopup()
    {
        var popup = UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        popup.SetContext(
            new InventoryContext
            {
                Icon = LoadIcon(ItemData.IconPath),
                Name = ItemData.Name,
                Count = ItemData.Count,
                Description = ItemData.Description,
                Actions = new List<(string label, UnityEngine.Events.UnityAction action)>
                {
                    (label: "Use", action: () => ItemData.UseItem()),
                }
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
