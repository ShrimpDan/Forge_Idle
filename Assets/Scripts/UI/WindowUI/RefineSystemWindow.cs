using UnityEngine;
using UnityEngine.UI;

public class RefineSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button materialSlotButton; // 재료 슬롯용 버튼
    [SerializeField] private Image materialIconImage;   // 슬롯에 표시할 아이콘 (에디터에서 연결)
    private ItemInstance selectedMaterial;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.RefineSystemWindow));

        materialSlotButton.onClick.RemoveAllListeners();
        materialSlotButton.onClick.AddListener(OnClickMaterialSlot);
    }

    private void OnClickMaterialSlot()
    {
        // 포지 인벤토리 팝업 열기 + 콜백 연결
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnMaterialSelected);
    }

    // 인벤토리 팝업에서 아이템을 선택했을 때 호출됨
    private void OnMaterialSelected(ItemInstance item)
    {
        selectedMaterial = item;

        // 슬롯에 아이콘 표시
        if (materialIconImage != null && item?.Data != null)
        {
            materialIconImage.sprite = IconLoader.GetIcon(item.Data.IconPath);
            materialIconImage.enabled = true;
        }
        // 추가 로직 필요시 여기서
    }

    public override void Open()
    {
        base.Open();
        // 필요 시 UI 초기화
    }

    public override void Close()
    {
        base.Close();
    }
}
