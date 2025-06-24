using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MineWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] Button exitBtn;
    [SerializeField] Transform mineSlotParent; // ���Ե��� ���� �θ�
    [SerializeField] GameObject mineSlotPrefab; // ������(�Ʒ��� ���� ����)

    private List<MineData> mineList;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineWindow));
        // �ӽ� ������ �ʱ�ȭ (�ǵ����� ���� ��)
        mineList = new List<MineData>();
        for (int i = 1; i <= 5; i++)
        {
            mineList.Add(new MineData
            {
                Name = $"���� {i}",
                DropSprites = new List<Sprite>() { null, null } // ��� ������ ������ �ֱ�
            });
        }
    }

    public override void Open()
    {
        base.Open();
        PopulateMines();
    }

    private void PopulateMines()
    {
        foreach (Transform child in mineSlotParent)
            Destroy(child.gameObject);

        foreach (var mine in mineList)
        {
            var go = Instantiate(mineSlotPrefab, mineSlotParent);
            var slot = go.GetComponent<MineListSlot>();
            slot.Setup(mine, OnSelectMine);
        }
    }

    private void OnSelectMine(MineData mine)
    {
        // MineDetailWindow ����
        var detail = UIManager.Instance.OpenUI<MineDetailWindow>(UIName.MineDetailWindow);
        detail.Setup(mine);
    }
}

[System.Serializable]
public class MineData
{
    public string Name;
    public List<Sprite> DropSprites; // ��� ������ 2��
    // ����: ��Ÿ ������(���̵�, etc)
}
