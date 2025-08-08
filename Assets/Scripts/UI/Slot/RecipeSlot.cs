using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class RecipeSlot : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text craftTimeText;
    [SerializeField] private TMP_Text craftCostText;
    [SerializeField] private TMP_Text sellCostText;
    [SerializeField] private Transform requiredListRoot;
    [SerializeField] private GameObject resourceSlotPrefab;
    [SerializeField] private Button selectButton;

    private CraftingData craftingData;
    private ItemDataLoader itemLoader;
    private Forge forge;
    private InventoryManager inventory;
    private Action onSelectCallback;

    public void Setup(
    CraftingData data,
    ItemDataLoader itemLoader,
    Forge forge,
    InventoryManager inventory,
    Action onSelect)
    {
        craftingData = data;
        this.itemLoader = itemLoader;
        this.forge = forge;
        this.inventory = inventory;
        this.onSelectCallback = onSelect;

        var itemData = itemLoader?.GetItemByKey(data.ItemKey);
 
        Sprite iconSprite = (itemData != null)
            ? IconLoader.GetIconByKey(data.ItemKey)
            : null;

        if (itemData != null && iconSprite == null)
            Debug.LogError($"[RecipeSlot] Icon not found for itemKey: {data.ItemKey} (type: {itemData.ItemType})");

        itemIcon.sprite = iconSprite;
        itemIcon.enabled = iconSprite != null;

        itemName.text = itemData != null ? itemData.Name : "";
        craftTimeText.text = $"제작 시간: {data.craftTime}초";
        craftCostText.text = $"제작 비용: {data.craftCost:N0}";
        sellCostText.text = $"판매가: {data.sellCost:N0}";

        foreach (Transform child in requiredListRoot)
            Destroy(child.gameObject);

        SetRequireResourceSlot();

        selectButton.interactable = true;
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    private void SetRequireResourceSlot()
    {
        if (craftingData == null) return;

        foreach (Transform child in requiredListRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (var req in craftingData.RequiredResources)
        {
            if (resourceSlotPrefab == null) continue;
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var slot = go.GetComponent<ResourceSlot>();
            if (slot == null) continue;
            var resItem = itemLoader?.GetItemByKey(req.ResourceKey);
            Sprite reqIconSprite = (resItem != null)
                ? IconLoader.GetIcon(resItem.ItemType, resItem.ItemKey)
                : null;

            int owned = 0;
            if (inventory != null)
            {
                if (inventory.ResourceList != null)
                    owned += inventory.ResourceList.Where(x => x.ItemKey == req.ResourceKey).Sum(x => x.Quantity);
                if (inventory.GemList != null)
                    owned += inventory.GemList.Where(x => x.ItemKey == req.ResourceKey).Sum(x => x.Quantity);
            }
            slot.Set(resItem.Name, reqIconSprite, owned, req.Amount);
        }
    }

    private void OnSelectButtonClicked()
    {
        onSelectCallback?.Invoke();
        SetRequireResourceSlot();
    }

    private void Reset()
    {
        if (!selectButton) selectButton = GetComponent<Button>();
    }
}
