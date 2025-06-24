using UnityEngine;
using UnityEngine.UI;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CraftWeaponWindow));
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
