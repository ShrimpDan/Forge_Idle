using UnityEngine;
using UnityEngine.UI;

public class RefineSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button materialSlotButton; // 빈 재료 슬롯 버튼

    private void Awake()
    {
        exitButton.onClick.AddListener(Close);
        materialSlotButton.onClick.AddListener(OnClickMaterialSlot);
    }

    private void OnClickMaterialSlot()
    {
        // 인벤토리 팝업을 연다 (팀원 코드와 호환)
        UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        // 실제 아이템 선택 및 후처리는 팀원 로직에 맞춤 (여기서는 팝업만 띄움)
    }

    public override void Open()
    {
        base.Open();
        // 슬롯 UI 초기화 등 추가 작업 필요시 여기에 작성
    }

    public override void Close()
    {
        base.Close();
    }
}
