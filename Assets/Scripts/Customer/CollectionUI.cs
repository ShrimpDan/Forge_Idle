using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : BaseUI
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefabs;
    [SerializeField] private Button exitBtn;


    private readonly Dictionary<RegualrCustomerData, CustomerSlotUI> slotDic = new();

    public override UIType UIType => UIType.Popup;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CollectionWindow));

        SettingSlots();
    }


    private void OnEnable()
    {
        CollectionBookManager.Instance.OnCustomerDiscovered += UpdateSlot;
    }
    private void OnDisable()
    {
        CollectionBookManager.Instance.OnCustomerDiscovered -= UpdateSlot;
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
        if (slotPrefabs == null)
        {
            Debug.Log("[CollectionUI] 설정 안됨");
            return;
        }
        if (slotParent == null)
        {
            return;
        }

        foreach (var data in CollectionBookManager.Instance.GetAllCustomerData())
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
            bool isDiscovered = CollectionBookManager.Instance.IsDiscovered(slot.Key);
            slot.Value.UpdateState(isDiscovered);
        }
    }


    private void UpdateSlot(RegualrCustomerData data)
    {
        Debug.Log($"[CollectionUI] UpdateSlot 호출됨: {data.customerName}");
        if (slotDic.TryGetValue(data, out var slot))
        {
            slot.UpdateState(true);
        }
        else
        {
            Debug.LogWarning($"[CollectionUI] slotDic에서 {data.customerName} 찾지 못함");
        }
    }

}
