using UnityEngine;
using UnityEngine.UI;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button inputWeaponSlotBtn; // 빈 무기 슬롯
    [SerializeField] private Image inputWeaponIcon;     // 빈 무기 슬롯 아이콘 (Image)
    [SerializeField] private Image resultWeaponIcon;    // 강화 후 무기 슬롯 아이콘 (Image)

    private ItemData selectedWeapon;

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);
        inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
    }

    private void OnClickInputWeaponSlot()
    {
        // 인벤토리 팝업만 연다 (실제 아이템 선택은 팀원이 담당)
        UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);

    }

    // 예시: 인벤토리 팝업에서 무기 선택 후 호출될 함수
    public void OnWeaponSelected(ItemData weapon)
    {
        selectedWeapon = weapon;
        inputWeaponIcon.sprite = LoadIcon(weapon.IconPath);      // 빈 무기 슬롯에 아이콘 표시
        resultWeaponIcon.sprite = LoadIcon(weapon.IconPath);     // 강화 후 슬롯에도 동일 아이콘 표시

        // 향후 resultWeaponIcon 옆에 "+1" 등 숫자 추가는 별도 Text로 후처리
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
