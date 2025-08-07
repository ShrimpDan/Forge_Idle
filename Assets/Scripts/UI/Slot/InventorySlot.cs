using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private UIManager uIManager;

    [SerializeField] private Image icon;
    [SerializeField] private Image slotBG;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject equippedIndicator;

    [Header("Enhance & Gem UI")]
    [SerializeField] private TextMeshProUGUI enhanceText;
    [SerializeField] private Transform gemRoot;
    [SerializeField] private GameObject gemIconSlotPrefab;

    private Button slotBtn;
    private List<GameObject> gemSlotInstances = new List<GameObject>();

    public ItemType ItemType { get; private set; }
    public ItemInstance SlotItem { get; private set; }

    public void Init(ItemInstance item)
    {
        slotBtn = GetComponent<Button>();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(() => OnClickSlot());

        SlotItem = item;
        nameText.text = item.Data.Name;
        icon.sprite = IconLoader.GetIconByKey(SlotItem.ItemKey);

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;

        if (item.Data.ItemType == ItemType.Weapon)
        {
            SetEquipped(SlotItem.IsEquipped);
            countText.gameObject.SetActive(false);

            UpdateEnhanceText(SlotItem.CurrentEnhanceLevel);

            UpdateGemSlots(SlotItem.GemSockets);
        }
        else
        {
            SetEquipped(false);
            countText.text = item.Quantity.ToString();
            if (enhanceText != null) enhanceText.text = "";
            ClearGemSlots();
        }
    }

    private void OnClickSlot()
    {
        if (SlotItem == null) return;

        SoundManager.Instance.Play("SFX_SystemClick");

        var ui = uIManager.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        ui.SetItemInfo(SlotItem);
    }

    public void SetEquipped(bool isEquipped)
    {
        equippedIndicator.SetActive(isEquipped);

        if (slotBG != null)
        {
            slotBG.color = isEquipped
                ? new Color(1f, 0.85f, 0.3f, 1f)
                : Color.white;
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

            // 색상 구간: 5, 8, 10, 13, 15
            if (enhanceLevel <= 5)
                enhanceText.color = Color.green;
            else if (enhanceLevel <= 8)
                enhanceText.color = new Color(0.28f, 0.53f, 1f); // 파랑 (RGB 72,136,255)
            else if (enhanceLevel <= 10)
                enhanceText.color = new Color(0.8f, 0.35f, 1f); // 보라
            else if (enhanceLevel <= 13)
                enhanceText.color = new Color(1f, 0.5f, 0f); // 주황
            else // 14, 15+
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
            {
                Destroy(go);
            }
            gemSlotInstances.Clear();
        }
    }
}
