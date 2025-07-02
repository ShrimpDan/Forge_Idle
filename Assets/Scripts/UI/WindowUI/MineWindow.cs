using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MineWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform mineSlotParent;
    [SerializeField] private GameObject mineSlotPrefab;

    private List<MineListSlot> mineSlots = new();
    private GameManager gameManager;
    private UIManager uIManager;
    private DataManger dataManager;
    private List<MineData> mineDataList = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;
        this.dataManager = gameManager?.DataManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.MineWindow));

        GenerateMineList();
    }

    private void GenerateMineList()
    {
        foreach (Transform child in mineSlotParent)
            Destroy(child.gameObject);
        mineSlots.Clear();

        var mineList = dataManager?.MineLoader?.DataList;
        var itemLoader = dataManager?.ItemLoader;
        if (mineList == null || itemLoader == null)
        {
            Debug.LogError("[MineWindow] 마인 데이터 로드 실패");
            return;
        }

        mineDataList = mineList;
        for (int i = 0; i < mineList.Count; i++)
        {
            var mine = mineList[i];
            GameObject go = Instantiate(mineSlotPrefab, mineSlotParent);
            MineListSlot slot = go.GetComponent<MineListSlot>();
            Sprite[] drops = new Sprite[2];
            for (int k = 0; k < 2; k++)
            {
                if (mine.RewardMineralKeys != null && k < mine.RewardMineralKeys.Count)
                {
                    var itemData = itemLoader.GetItemByKey(mine.RewardMineralKeys[k]);
                    drops[k] = itemData != null ? IconLoader.GetIcon(itemData.IconPath) : null;
                }
            }
            int slotIndex = i;
            slot.Init(mine.MineName, drops, () => OpenMineDetail(slotIndex));
            mineSlots.Add(slot);
        }
    }

    private void OpenMineDetail(int mineIdx)
    {
        if (mineIdx < 0 || mineIdx >= mineDataList.Count) return;
        var detailWin = uIManager.OpenUI<MineDetailWindow>(UIName.MineDetailWindow);
        detailWin.SetupMine(mineDataList[mineIdx], dataManager, gameManager, uIManager);
    }
}
