using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.QuestWindow));
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
