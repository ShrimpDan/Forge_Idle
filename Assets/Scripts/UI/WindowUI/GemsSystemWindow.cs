using UnityEngine;
using UnityEngine.UI;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button weaponInputBtn;
    [SerializeField] private Transform gemSlotRoot;
    [SerializeField] private GameObject gemSlotPrefab;

    private UIManager uiManager;
    private GameManager gameManager;
    private ItemInstance selectedWeapon;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.uiManager = uiManager;
        this.gameManager = gameManager;

        // 이벤트 연결 안전하게 항상!
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);

        ResetGemSlots();
    }

    void OpenWeaponInventoryPopup()
    {
        // 콜백 넘겨줌 (Popup 열릴 때마다 새로 연결)
        var popup = uiManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        ResetGemSlots();
    }

    private void ResetGemSlots()
    {
        foreach (Transform t in gemSlotRoot)
            Destroy(t.gameObject);

        if (selectedWeapon != null)
        {
            for (int i = 0; i < 3; i++)
            {
                var go = Instantiate(gemSlotPrefab, gemSlotRoot);
                var btn = go.GetComponent<Button>();
                int slotIdx = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnClickGemSlot(slotIdx));
            }
        }
    }

    private void OnClickGemSlot(int idx)
    {
        Debug.Log($"[GemsSystemWindow] Gem 슬롯 {idx} 클릭됨");
        // Gem 삽입 등 후처리
    }

    public override void Open()
    {
        base.Open();
        // 항상 초기화
        selectedWeapon = null;
        ResetGemSlots();

        // 이벤트 재연결
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);
    }

    public override void Close()
    {
        base.Close();
    }
}
