using UnityEngine;
using UnityEngine.UI;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button weaponInputBtn;

    private UIManager uiManager;
    private GameManager gameManager;

    private ItemInstance selectedWeapon;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.uiManager = uiManager;
        this.gameManager = gameManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);
        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);
    }

    // Forge_Inventory_Popup에서 무기 선택 콜백 받기
    private void OpenWeaponInventoryPopup()
    {
        var popup = uiManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    // 무기 선택 시 실행
    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        Debug.Log($"[GemSystemWindow] 무기 선택됨: {weapon?.Data?.Name}");
        // 슬롯 3개 생성 및 UI 업데이트 로직 추가
    }

    public override void Open()
    {
        base.Open();
        selectedWeapon = null;
    }

    public override void Close()
    {
        base.Close();
    }
}
