using UnityEngine;
using UnityEngine.UI;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button weaponInputBtn;
    [SerializeField] private Transform gemSlotRoot;
    [SerializeField] private GameObject gemSlotPrefab;

    private ItemInstance selectedWeapon;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);

        // �̺�Ʈ ���� �����ϰ� �׻�!
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);

        ResetGemSlots();
    }

    void OpenWeaponInventoryPopup()
    {
        // �ݹ� �Ѱ��� (Popup ���� ������ ���� ����)
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
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
        Debug.Log($"[GemsSystemWindow] Gem ���� {idx} Ŭ����");
        // Gem ���� �� ��ó��
    }

    public override void Open()
    {
        base.Open();
        // �׻� �ʱ�ȭ
        selectedWeapon = null;
        ResetGemSlots();

        // �̺�Ʈ �翬��
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
