using UnityEngine;
using UnityEngine.UI;

public class DungeonWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    [Header("Dungeon Slots")]
    [SerializeField] GameObject dungeonSlotPrefab;
    [SerializeField] Transform dungeonRoot;

    [Header("Scroll Bar")]
    [SerializeField] Scrollbar scrollBar;
    private const string DUNGEON_SCROLL_KEY = "DungeonScroll";

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

        scrollBar.value = PlayerPrefs.GetFloat(DUNGEON_SCROLL_KEY, 1f);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        PlayerPrefs.SetFloat(DUNGEON_SCROLL_KEY, scrollBar.value);
        base.Close();
    }
}
