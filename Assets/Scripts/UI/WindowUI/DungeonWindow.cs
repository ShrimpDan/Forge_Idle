using UnityEngine;
using UnityEngine.UI;

public class DungeonWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    [Header("Dungeon Slots")]
    [SerializeField] DungeonSlot[] dungeonSlots;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.DungeonWindow));

        foreach (var slot in dungeonSlots)
        {
            slot.Init(new DungeonData());
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
