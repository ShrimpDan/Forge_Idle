using UnityEngine;
using UnityEngine.UI;

public class UpgradeWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button inputWeaponSlotBtn;
    [SerializeField] private Image inputWeaponIcon;

    private ItemInstance selectedWeapon;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        inputWeaponSlotBtn.onClick.RemoveAllListeners();
        inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
    }

    private void OnClickInputWeaponSlot()
    {
        // 반드시 Forge_Inventory_Popup을 연다!
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

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
