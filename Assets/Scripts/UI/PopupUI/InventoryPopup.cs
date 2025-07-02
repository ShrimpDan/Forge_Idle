using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContext
{
    public Sprite Icon;
    public string Name;
    public int Count;
    public string Description;
}

public class InventoryPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI objectName;
    [SerializeField] private TextMeshProUGUI valueType;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button exitButton;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    private ItemInstance slotItem;
    private Button equipBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.InventoryPopup));
    }

    public void SetItemInfo(ItemInstance item)
    {
        slotItem = item;
        icon.sprite = IconLoader.GetIcon(item.Data.IconPath);
        objectName.text = item.Data.Name;
        value.text = item.Quantity.ToString("F0");

        string desc = item.Data.Description;

        switch (item.Data.ItemType)
        {
            case ItemType.Weapon:
                desc += $"\n\n<color=#00c3ff><b>▶ Stats</b></color>\n";
                desc += $"공격력: <b>{item.GetTotalAttack()}</b>\n";
                desc += $"공격 간격: <b>{item.GetTotalInterval()}초</b>\n";
                break;

            case ItemType.Gem:
                desc += $"\n\n<color=#ffcc00><b>▶ 효과</b></color>\n";
                desc += $"강화 배율: <b>{item.Data.GemStats.GemMultiplier:F1}x</b>\n";
                desc += $"{item.Data.GemStats.GemEffectDescription}";
                break;
        }

        description.text = desc;
        
        if (item.Data.ItemType == ItemType.Weapon)
        {
            CreateButton(item);
        }
    }

    private void CreateButton(ItemInstance item)
    {
        GameObject go = Instantiate(buttonPrefab, buttonContainer);
        equipBtn = go.GetComponent<Button>();
        var btnName = go.GetComponentInChildren<TextMeshProUGUI>();

        if (!item.IsEquipped)
            btnName.text = "Equip";
        else
            btnName.text = "UnEquip";

        equipBtn.onClick.AddListener(SetEquip);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    public void SetEquip()
    {
        if (equipBtn == null) return;
        var btnName = equipBtn.GetComponentInChildren<TextMeshProUGUI>();

        if (slotItem.IsEquipped)
        {
            slotItem.UnEquipItem();
            gameManager.Inventory.UnEquipItem(slotItem);
            btnName.text = "Equip";
        }
        else
        {
            slotItem.EquipItem();
            gameManager.Inventory.EquipItem(slotItem);
            btnName.text = "UnEquip";
        }
    }
}
