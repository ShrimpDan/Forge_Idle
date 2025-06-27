using UnityEngine;
using UnityEngine.UI;

public class Forge_Inventory_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private ForgeInventoryTab inventoryTab; // ���� �κ��丮 ��

    private System.Action<ItemInstance> weaponSelectCallback;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        // �κ��丮 �� �ʱ�ȭ
        inventoryTab.Init(gameManager, uiManager);
        inventoryTab.SetWeaponSlotCallback(OnWeaponSlotClicked);
    }

    // ���� ���� �ݹ� �ܺο��� ����
    public void SetWeaponSelectCallback(System.Action<ItemInstance> callback)
    {
        weaponSelectCallback = callback;
    }

    // ���� ���� Ŭ�� �� ����� ���� �Լ�
    private void OnWeaponSlotClicked(ItemInstance weapon)
    {
        weaponSelectCallback?.Invoke(weapon); // ���� ���� ����
        Close(); // ���� �� �˾� �ݱ�
    }
}
