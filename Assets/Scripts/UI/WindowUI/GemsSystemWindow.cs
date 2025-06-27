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

    // Forge_Inventory_Popup���� ���� ���� �ݹ� �ޱ�
    private void OpenWeaponInventoryPopup()
    {
        var popup = uiManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    // ���� ���� �� ����
    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        Debug.Log($"[GemSystemWindow] ���� ���õ�: {weapon?.Data?.Name}");
        // ���� 3�� ���� �� UI ������Ʈ ���� �߰�
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
