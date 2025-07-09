using UnityEngine;
using UnityEngine.UI;

public class DungeonWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    [Header("Dungeon Slots")]
    [SerializeField] GameObject dungeonSlotPrefab;
    [SerializeField] Transform dungeonRoot;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.DungeonWindow));

        if (dungeonRoot.childCount == 0)
        {
            foreach (var data in gameManager.DataManager.DungeonDataLoader.DungeonLists)
            {
                GameObject obj = Instantiate(dungeonSlotPrefab, dungeonRoot);

                if (obj.TryGetComponent(out DungeonSlot slot))
                {
                    slot.Init(gameManager, data);
                }
            }
        }
        
        else
        {
            foreach (Transform child in dungeonRoot)
            {
                if (child.TryGetComponent(out DungeonSlot slot))
                {
                    slot.SetUnlock();
                }
            }
        }
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }
}
