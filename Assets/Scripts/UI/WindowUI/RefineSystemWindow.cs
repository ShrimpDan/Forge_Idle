using UnityEngine;
using UnityEngine.UI;

public class RefineSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button materialSlotButton; // �� ��� ���� ��ư

    private void Awake()
    {
        exitButton.onClick.AddListener(Close);
        materialSlotButton.onClick.AddListener(OnClickMaterialSlot);
    }

    private void OnClickMaterialSlot()
    {
        // �κ��丮 �˾��� ���� (���� �ڵ�� ȣȯ)
        uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        // ���� ������ ���� �� ��ó���� ���� ������ ���� (���⼭�� �˾��� ���)
    }

    public override void Open()
    {
        base.Open();
        // ���� UI �ʱ�ȭ �� �߰� �۾� �ʿ�� ���⿡ �ۼ�
    }

    public override void Close()
    {
        base.Close();
    }
}
