using UnityEngine;
using UnityEngine.UI;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] Button exitBtn;
    [SerializeField] Button weaponInputBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);
    }

    void OpenWeaponInventoryPopup()
    {
        uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
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
