using UnityEngine;
using UnityEngine.UI;

public class UpgradeWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button inputWeaponSlotBtn; // 무기 입력 슬롯 버튼
    [SerializeField] private Image inputWeaponIcon;     // 무기 입력 슬롯에 표시할 Image

    private ItemInstance selectedWeapon; // 선택된 무기 보관용

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);
        inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
    }

    private void OnClickInputWeaponSlot()
    {
        // 인벤토리 팝업 열고, 무기 선택 콜백 연결
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    // 팝업에서 무기 선택 시 콜백
    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (inputWeaponIcon != null && weapon?.Data != null)
        {
            inputWeaponIcon.sprite = IconLoader.GetIcon(weapon.Data.IconPath);
            inputWeaponIcon.enabled = true;
        }
    }

    public override void Open()
    {
        base.Open();
        inputWeaponIcon.sprite = null;
        inputWeaponIcon.enabled = false;
    }

    public override void Close()
    {
        base.Close();
    }
}
