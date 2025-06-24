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

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.InventoryPopup));
    }

    public void SetItemInfo(ItemInstance item)
    {
        slotItem = item;
        icon.sprite = Resources.Load<Sprite>(item.Data.IconPath);
        objectName.text = item.Data.Name;
        value.text = item.Quantity.ToString("F0");

        string desc = item.Data.Description;

        switch (item.Data.ItemType)
        {
            case ItemType.Equipment:
                desc += $"\n\n<color=#00c3ff><b>▶ Stats</b></color>\n";
                desc += $"공격력: <b>{item.Data.EquipmentStats.Attack}</b>\n";
                desc += $"방어력: <b>{item.Data.EquipmentStats.Defense}</b>\n";
                desc += $"강화 최대치: <b>{item.Data.EquipmentStats.EnhanceMax}</b>";
                break;

            case ItemType.Gem:
                desc += $"\n\n<color=#ffcc00><b>▶ 효과</b></color>\n";
                desc += $"강화 배율: <b>{item.Data.GemStats.EnhanceMultiplier:F1}x</b>\n";
                desc += $"{item.Data.GemStats.EffectDescription}";
                break;
        }

        description.text = desc;
        
        if (item.Data.ItemType == ItemType.Equipment)
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
            btnName.text = "Equip";
        }
        else
        {
            slotItem.EquipItem();
            btnName.text = "UnEquip";
        }
    }
}
