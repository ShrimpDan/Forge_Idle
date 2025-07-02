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
    private DataManger dataManager;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;
        this.dataManager = gameManager?.DataManager;

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

        // 혹시 Data가 비어있으면 DataLoader에서
        if (selectedWeapon != null && selectedWeapon.Data == null && dataManager != null)
            selectedWeapon.Data = dataManager.ItemLoader.GetItemByKey(selectedWeapon.ItemKey);

        if (inputWeaponIcon)
        {
            if (selectedWeapon != null && selectedWeapon.Data != null)
            {
                inputWeaponIcon.sprite = IconLoader.GetIcon(selectedWeapon.Data.IconPath);
                inputWeaponIcon.enabled = true;
            }
            else
            {
                inputWeaponIcon.sprite = null;
                inputWeaponIcon.enabled = false;
            }
        }

        // 무기를 선택하면 비용을 계산해서 표시
        upgradeCost = CalcUpgradeCost(selectedWeapon);
        if (upgradeCostText)
            upgradeCostText.text = $"강화 비용: {upgradeCost} 골드";
    }

    // 강화 비용 계산
    private int CalcUpgradeCost(ItemInstance weapon)
    {
        if (weapon == null || weapon.Data == null)
            return 0;

        int level = weapon.CurrentEnhanceLevel;
        // 추후 강화 비용 로직 추가
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

        if (selectedWeapon != null && selectedWeapon.CanEnhance)
        {
            if (gameManager.Forge.UseGold(upgradeCost))
            {
                selectedWeapon.EnhanceItem();
                Debug.Log($"[UpgradeSystem] {selectedWeapon.Data.Name} 강화 성공! (레벨:{selectedWeapon.CurrentEnhanceLevel} 비용:{upgradeCost})");
            }
            else
                Debug.Log("[UpgradeSystem] 골드가 부족합니다");
        }
        else
            Debug.Log("[UpgradeSystem] 최대 강화입니다.");

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
