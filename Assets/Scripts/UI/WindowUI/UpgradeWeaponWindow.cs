using UnityEngine;
using UnityEngine.UI;

public class UpgradeWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button inputWeaponSlotBtn; // �� ���� ���� ��ư
    [SerializeField] private Image inputWeaponIcon;     // �� ���� ���� ������ (Image)

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);
        inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
    }

    private void OnClickInputWeaponSlot()
    {
        // �κ��丮 �˾�
        uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
    }

    public void OnWeaponSelected(ItemData weapon)
    {
        inputWeaponIcon.sprite = LoadIcon(weapon.IconPath);
    }

    private Sprite LoadIcon(string path)
    {
        Sprite icon = Resources.Load<Sprite>(path);
        return icon ? icon : null;
    }

    public override void Open()
    {
        base.Open();
        inputWeaponIcon.sprite = null;
    }

    public override void Close()
    {
        base.Close();
    }
}
