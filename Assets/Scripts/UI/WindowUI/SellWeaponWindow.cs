using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SellWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] SellWeaponSlot[] sellSlots;
    private Dictionary<CustomerJob, SellWeaponSlot> weaponDict;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SellWeaponWindow));

        weaponDict = new Dictionary<CustomerJob, SellWeaponSlot>();
        for (int i = 0; i < sellSlots.Length; i++)
        {
            var slot = sellSlots[i];
            weaponDict[(CustomerJob)i] = slot;

            slot.Init(gameManager);
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
