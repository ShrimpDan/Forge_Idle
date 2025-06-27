using UnityEngine;
using UnityEngine.UI;

public class Forge_Inventory_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private ForgeInventoryTab inventoryTab; // 전용 인벤토리 탭

    private System.Action<ItemInstance> weaponSelectCallback;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        // 인벤토리 탭 초기화
        inventoryTab.Init(gameManager, uiManager);
        inventoryTab.SetWeaponSlotCallback(OnWeaponSlotClicked);
    }

    // 무기 선택 콜백 외부에서 전달
    public void SetWeaponSelectCallback(System.Action<ItemInstance> callback)
    {
        weaponSelectCallback = callback;
    }

    // 무기 슬롯 클릭 시 실행될 내부 함수
    private void OnWeaponSlotClicked(ItemInstance weapon)
    {
        weaponSelectCallback?.Invoke(weapon); // 선택 무기 전달
        Close(); // 선택 후 팝업 닫기
    }
}
