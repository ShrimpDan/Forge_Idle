using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellWeaponSlot : MonoBehaviour
{
    private GameManager gameManager;
    private DataManager dataManager;
    private UIManager uIManager;

    [Header("UI Elements")]
    [SerializeField] private Button slotBtn;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Slot Type")]
    [SerializeField] CustomerJob slotType;

    public ItemData SlotItem { get; private set; }
    private CraftingData craftingData;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        dataManager = gameManager.DataManager;
        uIManager = gameManager.UIManager;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickButton);

        // string key = gameManager.Forge.SellingSystem.CraftingWeapon[slotType]?.ItemKey;

        // if(key != null)
        //     SetItem(key);
    }

    public void SetItem(string key)
    {
        SlotItem = dataManager.ItemLoader.GetItemByKey(key);
        craftingData = dataManager.CraftingLoader.GetDataByKey(key);
        icon.sprite = IconLoader.GetIconByPath(SlotItem.IconPath);
        priceText.text = craftingData.sellCost.ToString();
    }

    private void OnClickButton()
    {
        var ui = uIManager.OpenUI<SellWeaponPopup>(UIName.SellWeaponPopup);
        ui.Init(gameManager, uIManager);

        //ui.SetPopup(this, gameManager.Inventory.GetWeaponListByType(slotType));
    }   
}
