using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

        // 이 부분이 중요!
        var itemData = itemLoader?.GetItemByKey(data.ItemKey);
        // 아이콘 불러오기: data.ItemKey를 key로 활용
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

        // 재료 아이콘도 마찬가지
        foreach (Transform child in requiredListRoot)
            Destroy(child.gameObject);

        foreach (var req in data.RequiredResources)
        {
            if (resourceSlotPrefab == null) continue;
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var slot = go.GetComponent<ResourceSlot>();
            if (slot == null) continue;
            var resItem = itemLoader?.GetItemByKey(req.ResourceKey);
            Sprite reqIconSprite = (resItem != null)
                ? IconLoader.GetIconByPath(req.ResourceKey)
                : null;

            int owned = inventory?.ResourceList?.Find(x => x.ItemKey == req.ResourceKey)?.Quantity ?? 0;
            slot.Set(reqIconSprite, owned, req.Amount);
        }

        selectButton.interactable = true;
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectButtonClicked);
    }


    private void OnSelectButtonClicked()
    {
        onSelectCallback?.Invoke();
    }

    private void Reset()
    {
        if (!selectButton) selectButton = GetComponent<Button>();
    }
}
