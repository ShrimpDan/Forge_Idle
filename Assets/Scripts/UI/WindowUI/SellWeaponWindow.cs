using UnityEngine;
using UnityEngine.UI;

public class SellWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SellWeaponWindow));
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
