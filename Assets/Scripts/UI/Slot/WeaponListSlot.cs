using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponListSlot : MonoBehaviour
{
    private UIManager uIManager;
    private Forge forge;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI craftDurationText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button applyBtn;

    private ItemData itemData;
    private CraftingData craftingData;

    public void Init(GameManager gameManager, ItemData data, SellWeaponSlot slot)
    {
        if (data == null) return;
        uIManager = gameManager.UIManager;
        forge = gameManager.Forge;

        itemData = data;
        craftingData = gameManager.DataManager.CraftingLoader.GetDataByKey(data.ItemKey);

        SetUI();

        applyBtn.onClick.RemoveAllListeners();
        applyBtn.onClick.AddListener(() => OnClickApplyBtn(slot));
    }

    private void SetUI()
    {
        icon.sprite = IconLoader.GetIcon(itemData.IconPath);
        itemNameText.text = itemData.Name;
        craftDurationText.text = craftingData.craftTime.ToString("F1");
        priceText.text = craftingData.sellCost.ToString();

        
    }

    private void OnClickApplyBtn(SellWeaponSlot slot)
    {
        //forge.SellingSystem.SetCraftingItem(craftingData);
        slot.SetItem(itemData.ItemKey);

        uIManager.CloseUI(UIName.SellWeaponPopup);
    }
}
