using UnityEngine;
using UnityEngine.UI;

public class Forge_Inventory_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private ForgeInventoryTab inventoryTab;

    private System.Action<ItemInstance> weaponSelectCallback;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        inventoryTab.Init(gameManager, uiManager);
        inventoryTab.SetWeaponSlotCallback(OnWeaponSlotClicked);
    }

    public void SetWeaponSelectCallback(System.Action<ItemInstance> callback)
    {
        weaponSelectCallback = callback;
        if (inventoryTab != null)
            inventoryTab.SetWeaponSlotCallback(OnWeaponSlotClicked);
    }

    private void OnWeaponSlotClicked(ItemInstance weapon)
    {
        weaponSelectCallback?.Invoke(weapon);
        Close();
    }
}
