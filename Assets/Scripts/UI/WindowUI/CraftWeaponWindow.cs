using UnityEngine;
using UnityEngine.UI;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    [SerializeField] Button exitBtn;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
    }

    public override void Open()
    {
        base.Open();
        exitBtn.onClick.AddListener(Close);

    }

    public override void Close()
    {
        base.Close();
    }
}
