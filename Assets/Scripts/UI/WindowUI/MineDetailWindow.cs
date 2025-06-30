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
        // 광물 5개 생성
        foreach (Transform child in mineralSlotParent)
            Destroy(child.gameObject);
        mineralSlots.Clear();

        for (int i = 0; i < 5; i++)
        {
            GameObject go = Instantiate(mineralSlotPrefab, mineralSlotParent);
            MineralSlot slot = go.GetComponent<MineralSlot>();
            // 각 슬롯에 광물명/아이콘/콜백 설정
            slot.Init($"광물{i + 1}", null, () => OpenAssistantPopup(i));
            mineralSlots.Add(slot);
        }
    }

    private void OpenAssistantPopup(int mineralIndex)
    {
        // 인벤토리/어시스턴트 팝업
        Debug.Log($"광물 슬롯 팝업: {mineralIndex}");
        uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
    }
}
