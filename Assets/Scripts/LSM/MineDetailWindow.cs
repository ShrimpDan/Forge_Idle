using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MineDetailWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform mineralSlotParent;
    [SerializeField] private GameObject mineralSlotPrefab;

    private List<MineralSlot> mineralSlots = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineDetailWindow));
    }

    public void SetupMine(int mineId)
    {
        // ����: ���� 5�� ����
        foreach (Transform child in mineralSlotParent)
            Destroy(child.gameObject);
        mineralSlots.Clear();

        for (int i = 0; i < 5; i++)
        {
            GameObject go = Instantiate(mineralSlotPrefab, mineralSlotParent);
            MineralSlot slot = go.GetComponent<MineralSlot>();
            // ���� ������/������/���������� ��ü
            slot.Init($"����{i + 1}", null, () => OpenAssistantPopup(i));
            mineralSlots.Add(slot);
        }
    }

    private void OpenAssistantPopup(int mineralIndex)
    {
        // �κ��丮/���� �˾� UI ���� (���� ����)
        Debug.Log($"���� ��ġ �˾�: {mineralIndex}");
        uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        // �켱 �κ��丮 �˾� ��
    }
}
