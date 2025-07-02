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

    public void Setup(CraftingData data, ItemDataLoader itemLoader)
    {
        myCraftingData = data;
        myItemLoader = itemLoader;

        string mappedItemKey = MapItemKey(data.ItemKey);
        myItemData = itemLoader.GetItemByKey(mappedItemKey);

        if (myItemData == null)
        {
            Debug.LogError($"[RecipeSlot] ItemData를 찾을 수 없습니다: {data.ItemKey} (변환된 키: {mappedItemKey})");
            return;
        }

        itemIcon.sprite = Resources.Load<Sprite>(myItemData.IconPath);
        itemName.text = myItemData.Name;
        craftTimeText.text = $"제작시간: {data.craftTime}초";
        craftCostText.text = $"비용: {data.craftCost}";
        sellCostText.text = $"판매가: {data.sellCost}";

        foreach (Transform child in requiredListRoot)
            Destroy(child.gameObject);

        foreach (var req in data.RequiredResources)
        {
            var go = Instantiate(resourceSlotPrefab, requiredListRoot);
            var resourceIcon = go.transform.Find("ResourceIcon")?.GetComponent<Image>();
            var amountText = go.transform.Find("AmountText")?.GetComponent<TMP_Text>();

            string mappedResourceKey = MapItemKey(req.ResourceKey);
            var resItem = itemLoader.GetItemByKey(mappedResourceKey);

            if (resourceIcon != null && resItem != null)
                resourceIcon.sprite = Resources.Load<Sprite>(resItem.IconPath);
            if (amountText != null)
                amountText.text = $"x{req.Amount}";
        }
    }

    // 핵심 변환 함수: "crude_axe", "axe_crude" 등 → "weapon_axe_crude"
    private string MapItemKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;

        // 이미 정식 key면 바로 리턴
        if (key.StartsWith("weapon_") || key.StartsWith("resource_") || key.StartsWith("gem_") || key.StartsWith("ingot_"))
            return key;

        // 무기 타입 리스트 (데이터 json에 맞춰)
        string[] types = { "axe", "pickaxe", "sword", "dagger", "bow", "shield", "hoe" };

        var parts = key.Split('_');
        if (parts.Length == 2)
        {
            // crude_axe or axe_crude 스타일
            string p0 = parts[0];
            string p1 = parts[1];
            string type = types.Contains(p0) ? p0 : (types.Contains(p1) ? p1 : null);
            string quality = types.Contains(p0) ? p1 : (types.Contains(p1) ? p0 : null);
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(quality))
                return $"weapon_{type}_{quality}";
        }
        return key; // 기타는 변환 없음
    }

    public void SetSelectContext(ItemDataLoader itemLoader, Forge forge, Jang.InventoryManager inventory, Action onSelect)
    {
        myForge = forge;
        myInventory = inventory;
        onSelectCallback = onSelect;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectTryCraft);
    }

    private void OnSelectTryCraft()
    {
        if (myForge == null || myInventory == null || myCraftingData == null)
        {
            Debug.LogError("[RecipeSlot] 제작 시스템이 세팅되지 않았습니다!");
            return;
        }
        if (myForge.Gold < myCraftingData.craftCost)
        {
            Debug.Log($"[레시피] 골드 부족: {myForge.Gold}/{myCraftingData.craftCost}");
            return;
        }

        bool hasAll = true;
        foreach (var req in myCraftingData.RequiredResources)
        {
            string mappedResourceKey = MapItemKey(req.ResourceKey);
            int ownedAmount = myInventory.ResourceList
                .Where(x => x.ItemKey == mappedResourceKey)
                .Sum(x => x.Quantity);
            Debug.Log($"[자원체크] {mappedResourceKey} 필요:{req.Amount}, 보유:{ownedAmount}");
            if (ownedAmount < req.Amount)
            {
                Debug.Log($"[레시피] 재료 부족: {mappedResourceKey} 필요: {req.Amount}, 보유: {ownedAmount}");
                hasAll = false;
            }
        }
        if (!hasAll)
        {
            return;
        }
        onSelectCallback?.Invoke();
    }
}
