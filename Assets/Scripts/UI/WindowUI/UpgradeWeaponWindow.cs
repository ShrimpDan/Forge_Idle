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
    private GameManager gameManager;
    private UIManager uiManager;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.gameManager = gameManager;
        this.uiManager = uiManager;

        // 안전하게 리스너 등록
        if (exitBtn)
        {
            exitBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.AddListener(Close);
        }
        if (inputWeaponSlotBtn)
        {
            inputWeaponSlotBtn.onClick.RemoveAllListeners();
            inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
        }
        if (inputWeaponIcon)
        {
            inputWeaponIcon.sprite = null;
            inputWeaponIcon.enabled = false;
        }
        selectedWeapon = null;
    }

    private void OnClickInputWeaponSlot()
    {
        if (uiManager == null) return;

        var popup = uiManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        if (popup != null)
            popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (inputWeaponIcon)
        {
            if (weapon != null && weapon.Data != null)
            {
                inputWeaponIcon.sprite = IconLoader.GetIcon(weapon.Data.IconPath);
                inputWeaponIcon.enabled = true;
            }
            else
            {
                inputWeaponIcon.sprite = null;
                inputWeaponIcon.enabled = false;
            }
        }
    }

    public override void Open()
    {
        base.Open();
        selectedWeapon = null;
        if (inputWeaponIcon)
        {
            inputWeaponIcon.sprite = null;
            inputWeaponIcon.enabled = false;
        }
        if (inputWeaponSlotBtn)
            inputWeaponSlotBtn.interactable = true;
    }

    public override void Close()
    {
        base.Close();
        // 필요시 추가 정리
    }
}
