using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CollectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefabs;


    private readonly Dictionary<RegualrCustomerData, CustomerSlotUI> slotDic = new();


    private void Awake()
    {
        panel.SetActive(false);
    }


    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
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


    private void ResetAll()
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

}
