using UnityEngine;
using UnityEngine.UI;

public class DungeonWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.DungeonWindow));
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
