using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MineWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform mineSlotParent; // ���� ���� �θ�
    [SerializeField] private GameObject mineSlotPrefab; // MineListSlot ������

    private List<MineListSlot> mineSlots = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

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
            string mineName = $"���� {i}";
            Sprite[] drops = new Sprite[2]; // ���� ��� ������ ��������Ʈ�� ä���
            slot.Init(mineName, drops, () => OpenMineDetail(i));
            mineSlots.Add(slot);
        }
    }

    private void OpenMineDetail(int mineId)
    {
        var detailWin = uIManager.OpenUI<MineDetailWindow>(UIName.MineDetailWindow);
        detailWin.SetupMine(mineId);
    }
}
