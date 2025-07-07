using UnityEngine;
using UnityEngine.UI;
using System;

public class Forge_Inventory_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private ForgeInventoryTab inventoryTab;

    private Action<ItemInstance> weaponSelectCallback;
    private Action<ItemInstance> gemSelectCallback;
    private Action<ItemInstance> resourceSelectCallback;
    private Action<AssistantInstance> assistantSelectCallback;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.Forge_Inventory_Popup));

        inventoryTab.Init(gameManager, uiManager);
        ApplyCallbacks();
    }

    public void SetWeaponSelectCallback(Action<ItemInstance> callback)
    {
        weaponSelectCallback = callback; ApplyCallbacks();
    }
    public void SetGemSelectCallback(Action<ItemInstance> callback)
    {
        gemSelectCallback = callback; ApplyCallbacks();
    }
    public void SetResourceSelectCallback(Action<ItemInstance> callback)
    {
        resourceSelectCallback = callback; ApplyCallbacks();
    }
    public void SetTraineeSelectCallback(Action<AssistantInstance> callback)
    {
        assistantSelectCallback = callback; ApplyCallbacks();
    }

    private void ApplyCallbacks()
    {
        if (inventoryTab != null)
        {
            inventoryTab.SetSlotCallbacks(
                weaponSelectCallback,
                gemSelectCallback,
                resourceSelectCallback,
                assistantSelectCallback
            );
        }
    }

    public override void Close()
    {
        base.Close();
        weaponSelectCallback = null;
        gemSelectCallback = null;
        resourceSelectCallback = null;
        assistantSelectCallback = null;
    }
}
