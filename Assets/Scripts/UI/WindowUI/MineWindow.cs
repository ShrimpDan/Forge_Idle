using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MineWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform mineSlotParent; // 슬롯 생성 부모
    [SerializeField] private GameObject mineSlotPrefab; // MineListSlot 프리팹

    private List<MineListSlot> mineSlots = new();

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineWindow));

        GenerateMineList();
    }

    private void GenerateMineList()
    {
        foreach (Transform child in mineSlotParent)
            Destroy(child.gameObject);
        mineSlots.Clear();

        for (int i = 1; i <= 5; i++)
        {
            GameObject go = Instantiate(mineSlotPrefab, mineSlotParent);
            MineListSlot slot = go.GetComponent<MineListSlot>();
            string mineName = $"광산 {i}";
            Sprite[] drops = new Sprite[2]; // 실제 드랍 아이템 스프라이트로 채우기
            slot.Init(mineName, drops, () => OpenMineDetail(i));
            mineSlots.Add(slot);
        }
    }

    private void OpenMineDetail(int mineId)
    {
        var detailWin = UIManager.Instance.OpenUI<MineDetailWindow>(UIName.MineDetailWindow);
        detailWin.SetupMine(mineId);
    }
}
