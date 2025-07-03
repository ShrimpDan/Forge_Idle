using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellWeaponPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;
    [SerializeField] Button exitBtn;

    [Header("To Create SlotUI")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] Transform slotRoot;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SellWeaponPopup));
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    public void SetPopup(SellWeaponSlot weaponSlot, List<ItemData> itemDatas)
    {
        foreach (var data in itemDatas)
        {
            GameObject obj = Instantiate(slotPrefab, slotRoot);

            if (obj.TryGetComponent(out WeaponListSlot slot))
            {
                slot.Init(gameManager, data, weaponSlot);
            }
        }
    }
}
