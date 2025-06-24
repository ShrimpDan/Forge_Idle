using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MineWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] Button exitBtn;
    [SerializeField] Transform mineSlotParent; // 슬롯들을 놓을 부모
    [SerializeField] GameObject mineSlotPrefab; // 프리팹(아래에 예시 설명)

    private List<MineData> mineList;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineWindow));
        // 임시 데이터 초기화 (실데이터 연동 전)
        mineList = new List<MineData>();
        for (int i = 1; i <= 5; i++)
        {
            mineList.Add(new MineData
            {
                Name = $"광산 {i}",
                DropSprites = new List<Sprite>() { null, null } // 드랍 아이템 아이콘 넣기
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
        // MineDetailWindow 오픈
        var detail = UIManager.Instance.OpenUI<MineDetailWindow>(UIName.MineDetailWindow);
        detail.Setup(mine);
    }
}

[System.Serializable]
public class MineData
{
    public string Name;
    public List<Sprite> DropSprites; // 드랍 아이템 2개
    // 향후: 기타 데이터(난이도, etc)
}
