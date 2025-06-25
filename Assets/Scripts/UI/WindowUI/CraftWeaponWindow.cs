using UnityEngine;
using UnityEngine.UI;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button inputWeaponSlotBtn; // �� ���� ����
    [SerializeField] private Image inputWeaponIcon;     // �� ���� ���� ������ (Image)
    [SerializeField] private Image resultWeaponIcon;    // ��ȭ �� ���� ���� ������ (Image)

    private ItemData selectedWeapon;

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);
        inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
    }

    private void OnClickInputWeaponSlot()
    {
        // �κ��丮 �˾��� ���� (���� ������ ������ ������ ���)
        uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);

    }

    // ����: �κ��丮 �˾����� ���� ���� �� ȣ��� �Լ�
    public void OnWeaponSelected(ItemData weapon)
    {
        selectedWeapon = weapon;
        inputWeaponIcon.sprite = LoadIcon(weapon.IconPath);      // �� ���� ���Կ� ������ ǥ��
        resultWeaponIcon.sprite = LoadIcon(weapon.IconPath);     // ��ȭ �� ���Կ��� ���� ������ ǥ��

        // ���� resultWeaponIcon ���� "+1" �� ���� �߰��� ���� Text�� ��ó��
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
        resultWeaponIcon.sprite = null;
        selectedWeapon = null;
    }

    public override void Close()
    {
        base.Close();
    }
}
