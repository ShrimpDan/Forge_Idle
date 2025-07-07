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


    private readonly List<CustomerSlotUI> slots = new();    // 슬롯 참조(파괴 시 정리)

    public override UIType UIType => UIType.Popup;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CollectionWindow));

    }

    public void OnEnable()
    {
        BuildSlots();
    }

    public override void Open()
    {
        panel.SetActive(true);
    }

    public override void Close()
    {
        panel.SetActive(false);
    }
    private void BuildSlots()
    {
        // 1) 기존 슬롯 제거
        foreach (var s in slots)
        {
            if (s) Destroy(s.gameObject);
        }
        slots.Clear();

        // 2) 데이터 기준으로 새 슬롯 생성
        foreach (var data in CollectionBookManager.Instance.GetAllCustomerData())
        {
            var go = Instantiate(slotPrefabs, slotParent);
            var slot = go.GetComponent<CustomerSlotUI>();
            slot.Initialize(data);  // 내부에서 발견 여부 판단 & 실루엣/아이콘 표시
            slots.Add(slot);
        }
    }
}
