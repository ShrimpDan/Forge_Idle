using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button inputWeaponSlotBtn;
    [SerializeField] private Image inputWeaponIcon;
    [SerializeField] private TMP_Text upgradeCostText;     
    [SerializeField] private Button executeBtn;            

    private ItemInstance selectedWeapon;
    private int upgradeCost = 0; 
    private GameManager gameManager;
    private UIManager uIManager;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.gameManager = gameManager;
        this.uIManager = uiManager;

        // 버튼 리스너 등록
        if (exitBtn)
        {
            exitBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.UpgradeWeaponWindow));
        }
        if (inputWeaponSlotBtn)
        {
            inputWeaponSlotBtn.onClick.RemoveAllListeners();
            inputWeaponSlotBtn.onClick.AddListener(OnClickInputWeaponSlot);
        }
        if (executeBtn)
        {
            executeBtn.onClick.RemoveAllListeners();
            executeBtn.onClick.AddListener(OnClickExecuteUpgrade);
        }
        ResetUI();
    }

    private void OnClickInputWeaponSlot()
    {
        if (uIManager == null) return;
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
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
        // 무기를 선택하면 비용을 계산해서 표시
        upgradeCost = CalcUpgradeCost(weapon);
        if (upgradeCostText)
            upgradeCostText.text = $"강화 비용: {upgradeCost} 골드";
    }

    // 강화 비용 계산
    private int CalcUpgradeCost(ItemInstance weapon)
    {
        if (weapon == null || weapon.Data == null)
            return 0;
        int level = weapon.EnhanceLevel;
        return Mathf.Max(1000, (level + 1) * 1000);
    }

    private void OnClickExecuteUpgrade()
    {
        if (selectedWeapon == null)
        {
            Debug.LogWarning("[UpgradeSystem] 무기를 먼저 선택하세요!");
            return;
        }
        if (gameManager?.Forge == null)
        {
            Debug.LogError("[UpgradeSystem] Forge 인스턴스가 없습니다!");
            return;
        }

        if (gameManager.Forge.Gold < upgradeCost)
        {
            Debug.LogWarning("[UpgradeSystem] 골드가 부족합니다!");
            return;
        }

        // 비용 차감
        gameManager.Forge.AddGold(-upgradeCost);

        // 실제 무기 업그레이드 처리
        Debug.Log($"[UpgradeSystem] {selectedWeapon.Data.Name} 강화 성공! (레벨:{selectedWeapon.EnhanceLevel + 1} 비용:{upgradeCost})");

        // 여기서 실제로 업그레이드 수치 증가시키는 부분

        // 성공 후 UI 리셋 또는 재계산
        OnWeaponSelected(selectedWeapon);
    }

    private void ResetUI()
    {
        selectedWeapon = null;
        if (inputWeaponIcon)
        {
            inputWeaponIcon.sprite = null;
            inputWeaponIcon.enabled = false;
        }
        if (upgradeCostText)
            upgradeCostText.text = "강화 비용: -";
    }

    public override void Open()
    {
        base.Open();
        ResetUI();
        if (inputWeaponSlotBtn)
            inputWeaponSlotBtn.interactable = true;
    }

    public override void Close()
    {
        base.Close();
    }
}
