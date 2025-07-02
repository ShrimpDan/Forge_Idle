using System.Collections.Generic;
using UnityEngine;

public class CollectionUI : BaseUI
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefabs;


    private readonly Dictionary<RegualrCustomerData, CustomerSlotUI> slotDic = new();

    public override UIType UIType => UIType.Popup;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
      

        SettingSlots();
        CollectionBook.Instance.OnCustomerDiscovered += UpdateSlot;
    }


    public override void Open()
    {
        panel.SetActive(true);
        UpdateAllSlots(); //최신화
    }

    public override void Close()
    {
        panel.SetActive(false);
    }

    private void SettingSlots()
    {
        foreach (var data in CollectionBook.Instance.GetAllRegularCutsomer())
        {
            GameObject go = Instantiate(slotPrefabs, slotParent);
            var slot = go.GetComponent<CustomerSlotUI>();
            slot.Initialize(data);
            slotDic[data] = slot;
        }
    }


    private void UpdateAllSlots()
    {
        foreach (var slot in slotDic)
        {
            bool isDiscovered = CollectionBook.Instance.IsDiscovered(slot.Key);
            slot.Value.UpdateState(isDiscovered);
        }
    }

    private void ResetSlot(RegualrCustomerData data)
    {
        if (slotDic.TryGetValue(data, out var slot))
        {
            slot.UpdateState(true);
        }
    }


    private void UpdateSlot(RegualrCustomerData data)
    {
        if (slotDic.TryGetValue(data, out var slot))
        {
            slot.UpdateState(true);
        }
        
    }

}
