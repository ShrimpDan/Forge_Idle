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

    private ItemData myItemData;
    private CraftingData myCraftingData;
    private ItemDataLoader myItemLoader;
    private Forge myForge;
    private Jang.InventoryManager myInventory;
    private Action onSelectCallback;

    public void Setup(
        CraftingData data,
        ItemDataLoader itemLoader,
        Forge forge,
        Jang.InventoryManager inventory,
        Action onSelect)
    {
        myCraftingData = data;
        myItemLoader = itemLoader;
        myForge = forge;
        myInventory = inventory;
        onSelectCallback = onSelect;

        // 아이템 정보 세팅
        string mappedItemKey = MapItemKey(data.ItemKey);
        myItemData = itemLoader.GetItemByKey(mappedItemKey);
        itemIcon.sprite = myItemData != null ? Resources.Load<Sprite>(myItemData.IconPath) : null;
        itemIcon.enabled = myItemData != null && itemIcon.sprite != null;
        itemName.text = myItemData != null ? myItemData.Name : "";
        craftTimeText.text = $"제작시간: {data.craftTime}초";
        craftCostText.text = $"비용: {data.craftCost:N0}";
        sellCostText.text = $"판매가: {data.sellCost:N0}";

        // 기존 재료 슬롯 제거
        for (int i = requiredListRoot.childCount - 1; i >= 0; i--)
            Destroy(requiredListRoot.GetChild(i).gameObject);

        bool canCraft = true;

        foreach (var req in data.RequiredResources)
        {
            Debug.Log($"리소스 슬롯 생성: {req.ResourceKey} x {req.Amount}");
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var slot = go.GetComponent<ResourceSlot>();
            if (slot == null)
            {
                Debug.LogError("ResourceSlot 프리팹에 ResourceSlot 컴포넌트가 없음!");
                continue;
            }
            string reqKey = MapItemKey(req.ResourceKey);
            var resItem = itemLoader.GetItemByKey(reqKey);
            Sprite iconSprite = null;
            if (resItem != null)
                iconSprite = Resources.Load<Sprite>(resItem.IconPath);
            int owned = myInventory?.ResourceList
                .Where(x => x.ItemKey == reqKey)
                .Sum(x => x.Quantity) ?? 0;
            slot.Set(iconSprite, owned, req.Amount);
            if (owned < req.Amount)
                canCraft = false;
        }

        // 골드 체크
        bool enoughGold = myForge != null && myForge.Gold >= myCraftingData.craftCost;
        selectButton.interactable = canCraft && enoughGold;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectTryCraft);
    }

    private string MapItemKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        if (key.StartsWith("weapon_") || key.StartsWith("resource_") || key.StartsWith("gem_") || key.StartsWith("ingot_"))
            return key;
        string[] types = { "axe", "pickaxe", "sword", "dagger", "bow", "shield", "hoe" };
        var parts = key.Split('_');
        if (parts.Length == 2)
        {
            string p0 = parts[0];
            string p1 = parts[1];
            string type = types.Contains(p0) ? p0 : (types.Contains(p1) ? p1 : null);
            string quality = types.Contains(p0) ? p1 : (types.Contains(p1) ? p0 : null);
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(quality))
                return $"weapon_{type}_{quality}";
        }
        return key;
    }

    private void OnSelectTryCraft()
    {
        if (myForge == null || myInventory == null || myCraftingData == null)
            return;
        if (myForge.Gold < myCraftingData.craftCost)
            return;
        foreach (var req in myCraftingData.RequiredResources)
        {
            string mappedResourceKey = MapItemKey(req.ResourceKey);
            int ownedAmount = myInventory.ResourceList
                .Where(x => x.ItemKey == mappedResourceKey)
                .Sum(x => x.Quantity);
            if (ownedAmount < req.Amount)
                return;
        }
        onSelectCallback?.Invoke();
    }
}
