using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MineDetailWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] Button exitBtn;
    [SerializeField] TMP_Text titleText;
    [SerializeField] Transform mineralSlotParent;
    [SerializeField] GameObject mineralSlotPrefab; // �Ʒ� ��ũ��Ʈ ����

    private MineData mineData;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineDetailWindow));
    }

    public void Setup(MineData data)
    {
        mineData = data;
        titleText.text = data.Name;
        PopulateMineralSlots();
    }

    private void PopulateMineralSlots()
    {
        foreach (Transform child in mineralSlotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < 5; i++)
        {
            var go = Instantiate(mineralSlotPrefab, mineralSlotParent);
            var slot = go.GetComponent<MineralSlot>();
            slot.Setup($"���� {i + 1}", null, OnClickAssignAssistant);
        }
    }

    // ���� ����(�˾� ����)
    private void OnClickAssignAssistant(MineralSlot slot)
    {
        UIManager.Instance.OpenUI<BaseUI>(UIName.AssistantPopup); // ���� �κ��丮 �˾� ����
        // ����: ���� �ݹ� �Ѱ��ָ� ��
    }
}
