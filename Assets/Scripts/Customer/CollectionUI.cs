using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : BaseUI
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefabs;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TextMeshProUGUI totalBounsText;

    private Button slotButton;


    private readonly List<CustomerSlotUI> slots = new();    // 슬롯 참조(파괴 시 정리)

    public override UIType UIType => UIType.Window;

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
        UpdateTotalBounsUI();
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
        foreach (var data in GameManager.Instance.CollectionManager.GetAllCustomerData())
        {
            var go = Instantiate(slotPrefabs, slotParent);
            var slot = go.GetComponent<CustomerSlotUI>();

           

            var collectionData = GameManager.Instance.CollectionManager.GetCollectionData(data.Key);
            slot.Initialize(data, collectionData);

            slots.Add(slot);
        }
    }

    private void UpdateTotalBounsUI()
    {
        float total = GameManager.Instance.CollectionManager.GetTotalGoldBonus();
        totalBounsText.text = $"{total * 100f:F0}%";
    }

}
