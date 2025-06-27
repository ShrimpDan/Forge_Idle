using UnityEngine;
using UnityEngine.UI;
using System;

public class Forge_Inventory_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private ForgeInventoryTab inventoryTab;

    private Action<ItemInstance> weaponSelectCallback;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);

        if (exitBtn != null)
        {
            exitBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.Forge_Inventory_Popup));
        }

        if (inventoryTab != null)
        {
            inventoryTab.Init(gameManager, uiManager);
            inventoryTab.SetWeaponSlotCallback(OnWeaponSlotClicked);
        }
    }

    public void SetWeaponSelectCallback(Action<ItemInstance> callback)
    {
        weaponSelectCallback = callback;
        if (inventoryTab != null)
            inventoryTab.SetWeaponSlotCallback(OnWeaponSlotClicked);
    }

    private void OnWeaponSlotClicked(ItemInstance weapon)
    {
        weaponSelectCallback?.Invoke(weapon);
        uIManager.CloseUI(UIName.Forge_Inventory_Popup);
    }

    public override void Close()
    {
        base.Close();
        weaponSelectCallback = null;
    }
}
