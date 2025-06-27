using UnityEngine;
using UnityEngine.UI;

public class SellWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button selectWeaponBtn;    // ���� ���� ��ư
    [SerializeField] private Image selectedWeaponIcon;  // ���� ���� ������

    private ItemInstance selectedWeapon;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SellWeaponWindow));

        if (selectWeaponBtn != null)
        {
            selectWeaponBtn.onClick.RemoveAllListeners();
            selectWeaponBtn.onClick.AddListener(OnClickSelectWeapon);
        }
    }

    private void OnClickSelectWeapon()
    {
        // �κ��丮 �˾� ����, ���� ���� �ݹ� ���
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (selectedWeaponIcon != null && weapon?.Data != null)
        {
            selectedWeaponIcon.sprite = IconLoader.GetIcon(weapon.Data.IconPath);
            selectedWeaponIcon.enabled = true;
        }
    }

    public override void Open()
    {
        base.Open();
        if (selectedWeaponIcon != null)
            selectedWeaponIcon.enabled = false;
    }

    public override void Close()
    {
        base.Close();
    }
}
