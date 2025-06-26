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
        myItemData = itemLoader.GetItemByKey(data.ItemKey);

        if (myItemData == null)
        {
            Debug.LogError("[RecipeSlot] ItemData가 없습니다: " + data.ItemKey);
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
            var resItem = itemLoader.GetItemByKey(req.ResourceKey);
            if (resourceIcon != null && resItem != null)
                resourceIcon.sprite = Resources.Load<Sprite>(resItem.IconPath);
            if (amountText != null)
                amountText.text = $"x{req.Amount}";
        }
    }

    // 셋팅: 인벤토리, Forge, 콜백
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
            Debug.LogError("[RecipeSlot] 제작 시스템 연결 안됨!");
            return;
        }
        if (myForge.Gold < myCraftingData.craftCost)
        {
            Debug.Log($"[제작불가] 골드 부족: {myForge.Gold}/{myCraftingData.craftCost}");
            return;
        }

        bool hasAll = true;
        foreach (var req in myCraftingData.RequiredResources)
        {
            int ownedAmount = myInventory.ResourceList.Where(x => x.ItemKey == req.ResourceKey).Sum(x => x.Quantity);
            if (ownedAmount < req.Amount)
            {
                Debug.Log($"[제작불가] 재료 부족: {req.ResourceKey} 필요: {req.Amount}, 보유: {ownedAmount}");
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
