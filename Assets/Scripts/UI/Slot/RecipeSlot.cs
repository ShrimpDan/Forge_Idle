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

        // 아이템 정보 표시
        var myItemData = itemLoader?.GetItemByKey(data.ItemKey);
        itemIcon.sprite = myItemData != null ? IconLoader.GetIconByPath(myItemData.IconPath) : null;
        itemIcon.enabled = myItemData != null && itemIcon.sprite != null;
        itemName.text = myItemData != null ? myItemData.Name : "";
        craftTimeText.text = $"제작 시간: {data.craftTime}초";
        craftCostText.text = $"제작 비용: {data.craftCost:N0}";
        sellCostText.text = $"판매가: {data.sellCost:N0}";

        // 리소스 슬롯 갱신
        foreach (Transform child in requiredListRoot)
            Destroy(child.gameObject);

        foreach (var req in data.RequiredResources)
        {
            if (resourceSlotPrefab == null) continue;
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var slot = go.GetComponent<ResourceSlot>();
            if (slot == null) continue;
            var resItem = itemLoader?.GetItemByKey(req.ResourceKey);
            Sprite iconSprite = resItem != null ? IconLoader.GetIconByPath(resItem.IconPath) : null;
            int owned = inventory?.ResourceList?.Find(x => x.ItemKey == req.ResourceKey)?.Quantity ?? 0;
            slot.Set(iconSprite, owned, req.Amount);
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
