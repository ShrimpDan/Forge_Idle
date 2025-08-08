using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ForgeInventorySlot : MonoBehaviour
{
    private UIManager uIManager;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;


    [Header("Enhance & Gem UI")]
    [SerializeField] private TextMeshProUGUI enhanceText;
    [SerializeField] private Transform gemRoot;
    [SerializeField] private GameObject gemIconSlotPrefab;

    private Button slotBtn;
    private ItemInstance item;
    private Action<ItemInstance> onSlotClick;
    private List<GameObject> gemSlotInstances = new List<GameObject>();

    public void Init(ItemInstance item, Action<ItemInstance> onClick)
    {
        this.item = item;
        this.onSlotClick = onClick;

        slotBtn = GetComponent<Button>();
        if (slotBtn == null)
            slotBtn = gameObject.AddComponent<Button>();

        icon.sprite = item?.Data != null ? IconLoader.GetIconByKey(item.ItemKey) : null;
        icon.enabled = (icon.sprite != null);

        countText.text = (item != null) ? item.Quantity.ToString() : "";

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);


        UpdateEnhanceText(item?.CurrentEnhanceLevel ?? 0);


        UpdateGemSlots(item?.GemSockets);
    }

    private void OnClickSlot()
    {
        if (onSlotClick != null)
            onSlotClick(item);
        else if (uIManager != null && item != null)
        {
            var popup = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
            popup.SetItemInfo(item);
        }
    }

    private void UpdateEnhanceText(int enhanceLevel)
    {
        if (enhanceText == null)
            return;

        if (enhanceLevel > 0)
        {
            enhanceText.gameObject.SetActive(true);
            enhanceText.text = $"+{enhanceLevel}";

            // 5���� ��ȭ ����
            if (enhanceLevel <= 5)
                enhanceText.color = Color.green;
            else if (enhanceLevel <= 8)
                enhanceText.color = new Color(0.28f, 0.53f, 1f); // �Ķ�
            else if (enhanceLevel <= 10)
                enhanceText.color = new Color(0.8f, 0.35f, 1f); // ����
            else if (enhanceLevel <= 13)
                enhanceText.color = new Color(1f, 0.5f, 0f); // ��Ȳ
            else
                enhanceText.color = Color.red;
        }
        else
        {
            enhanceText.text = "";
            enhanceText.gameObject.SetActive(false);
        }
    }

    private void UpdateGemSlots(List<ItemInstance> gems)
    {
        ClearGemSlots();

        if (gemRoot == null || gemIconSlotPrefab == null || gems == null)
            return;

        for (int i = 0; i < gems.Count; i++)
        {
            var gem = gems[i];
            if (gem == null || gem.Data == null)
                continue;

            var go = Instantiate(gemIconSlotPrefab, gemRoot);
            var img = go.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = IconLoader.GetIcon(gem.Data.ItemType, gem.Data.ItemKey);
                img.enabled = true;
            }
            gemSlotInstances.Add(go);
        }
    }

    private void ClearGemSlots()
    {
        if (gemSlotInstances != null)
        {
            foreach (var go in gemSlotInstances)
                Destroy(go);
            gemSlotInstances.Clear();
        }
    }
}
