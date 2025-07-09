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

        // 아이템 데이터 체크
        var myItemData = itemLoader?.GetItemByKey(data.ItemKey);
        if (myItemData == null)
        {
            Debug.LogError($"[RecipeSlot] ItemData not found for key: {data.ItemKey}");
        }
        itemIcon.sprite = myItemData != null ? IconLoader.GetIcon(myItemData.IconPath) : null;
        itemIcon.enabled = myItemData != null && itemIcon.sprite != null;
        itemName.text = myItemData != null ? myItemData.Name : "";
        craftTimeText.text = $"제작시간: {data.craftTime}초";
        craftCostText.text = $"비용: {data.craftCost:N0}";
        sellCostText.text = $"판매가: {data.sellCost:N0}";

        // 기존 자식 제거
        foreach (Transform child in requiredListRoot)
            Destroy(child.gameObject);

        bool canCraft = true;

        foreach (var req in data.RequiredResources)
        {
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var slot = go.GetComponent<ResourceSlot>();
            if (slot == null)
            {
                Debug.LogError("[RecipeSlot] ResourceSlot 프리팹에 ResourceSlot 컴포넌트가 없음!");
                continue;
            }

            var resItem = itemLoader?.GetItemByKey(req.ResourceKey);
            if (resItem == null)
            {
                Debug.LogError($"[RecipeSlot] Resource item not found for key: {req.ResourceKey}");
            }
            Sprite iconSprite = resItem != null ? IconLoader.GetIcon(resItem.IconPath) : null;

            int owned = 0;
            if (myInventory != null)
            {
                owned = myInventory.ResourceList
                    .Where(x => x.ItemKey == req.ResourceKey)
                    .Sum(x => x.Quantity);
            }
            else
            {
                Debug.LogError("[RecipeSlot] myInventory is null!");
            }

            slot.Set(iconSprite, owned, req.Amount);
            if (owned < req.Amount) canCraft = false;
        }

        bool enoughGold = myForge != null && myForge.Gold >= myCraftingData.craftCost;
        selectButton.interactable = canCraft && enoughGold;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelectCallback?.Invoke());
    }
}
