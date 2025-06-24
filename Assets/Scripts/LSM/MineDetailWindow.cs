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

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineDetailWindow));
    }

    public void SetupMine(int mineId)
    {
        // 예시: 광물 5개 생성
        foreach (Transform child in mineralSlotParent)
            Destroy(child.gameObject);
        mineralSlots.Clear();

        for (int i = 0; i < 5; i++)
        {
            GameObject go = Instantiate(mineralSlotPrefab, mineralSlotParent);
            MineralSlot slot = go.GetComponent<MineralSlot>();
            // 실제 광물명/아이콘/조수정보로 대체
            slot.Init($"광물{i + 1}", null, () => OpenAssistantPopup(i));
            mineralSlots.Add(slot);
        }
    }

    private void OpenAssistantPopup(int mineralIndex)
    {
        // 인벤토리/조수 팝업 UI 연결 (추후 구현)
        Debug.Log($"조수 배치 팝업: {mineralIndex}");
        UIManager.Instance.OpenUI<InventoryPopup>;
        // 우선 인벤토리 팝업 염
        // UIManager.Instance.OpenUI<AssistantPopup>(UIName.AssistantPopup);
    }
}
