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
            int owned = inventory?.ResourceList?.Find(x => x.ItemKey == req.ResourceKey)?.Quantity ?? 0;
            slot.Set(iconSprite, owned, req.Amount);
            if (owned < req.Amount) canCraft = false;
        }

        // 골드 체크
        bool enoughGold = forge != null && forge.Gold >= craftingData.craftCost;

        selectButton.interactable = canCraft && enoughGold;
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    private void OnSelectButtonClicked()
    {
        Debug.Log($"[RecipeSlot] RecipeButton 클릭됨! {name}");
        onSelectCallback?.Invoke();
    }

    // 인스펙터에서 미할당시 에러 방지
    private void Reset()
    {
        if (!selectButton) selectButton = GetComponent<Button>();
    }
}
