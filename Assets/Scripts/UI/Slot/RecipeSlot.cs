using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class RecipeSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text craftTimeText;
    [SerializeField] private TMP_Text craftCostText;
    [SerializeField] private TMP_Text sellCostText;
    [SerializeField] private Transform requiredListRoot;
    [SerializeField] private GameObject resourceSlotPrefab;
    [SerializeField] private Button selectButton;

    private CraftingData myCraftingData;
    private ItemDataLoader myItemLoader;
    private Forge myForge;
    private InventoryManager myInventory;
    private Action onSelectCallback;

    public void Setup(
        CraftingData data,
        ItemDataLoader itemLoader,
        Forge forge,
        InventoryManager inventory,
        Action onSelect)
    {
        myCraftingData = data;
        myItemLoader = itemLoader;
        myForge = forge;
        myInventory = inventory;
        onSelectCallback = onSelect;

        var myItemData = itemLoader?.GetItemByKey(data.ItemKey);
        itemIcon.sprite = myItemData != null ? IconLoader.GetIcon(myItemData.IconPath) : null;
        itemIcon.enabled = myItemData != null && itemIcon.sprite != null;
        itemName.text = myItemData != null ? myItemData.Name : "";
        craftTimeText.text = $"제작시간: {data.craftTime}초";
        craftCostText.text = $"비용: {data.craftCost:N0}";
        sellCostText.text = $"판매가: {data.sellCost:N0}";

        // 리소스 슬롯 갱신
        foreach (Transform child in requiredListRoot)
            Destroy(child.gameObject);

        bool canCraft = true;
        foreach (var req in data.RequiredResources)
        {
            if (resourceSlotPrefab == null)
            {
                Debug.LogError("[RecipeSlot] resourceSlotPrefab 연결 필요!");
                continue;
            }
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var slot = go.GetComponent<ResourceSlot>();
            if (slot == null)
            {
                Debug.LogError("[RecipeSlot] ResourceSlot 컴포넌트 없음!");
                continue;
            }
            var resItem = itemLoader?.GetItemByKey(req.ResourceKey);
            Sprite iconSprite = resItem != null ? IconLoader.GetIcon(resItem.IconPath) : null;
            int owned = myInventory?.ResourceList?.Where(x => x.ItemKey == req.ResourceKey).Sum(x => x.Quantity) ?? 0;
            slot.Set(iconSprite, owned, req.Amount);
            if (owned < req.Amount) canCraft = false;
        }

        bool enoughGold = myForge != null && myForge.Gold >= myCraftingData.craftCost;
        selectButton.interactable = canCraft && enoughGold;
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelectCallback?.Invoke());
    }
}
