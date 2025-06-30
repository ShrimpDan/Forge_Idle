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
            Debug.LogError("[RecipeSlot] ItemData�� �����ϴ�: " + data.ItemKey);
            return;
        }

        itemIcon.sprite = Resources.Load<Sprite>(myItemData.IconPath);
        itemName.text = myItemData.Name;
        craftTimeText.text = $"���۽ð�: {data.craftTime}��";
        craftCostText.text = $"���: {data.craftCost}";
        sellCostText.text = $"�ǸŰ�: {data.sellCost}";

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

    // ����: �κ��丮, Forge, �ݹ�
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
            Debug.LogError("[RecipeSlot] ���� �ý��� ���� �ȵ�!");
            return;
        }
        if (myForge.Gold < myCraftingData.craftCost)
        {
            Debug.Log($"[���ۺҰ�] ��� ����: {myForge.Gold}/{myCraftingData.craftCost}");
            return;
        }

        bool hasAll = true;
        foreach (var req in myCraftingData.RequiredResources)
        {
            int ownedAmount = myInventory.ResourceList.Where(x => x.ItemKey == req.ResourceKey).Sum(x => x.Quantity);
            Debug.Log($"[���üũ] {req.ResourceKey} �ʿ�:{req.Amount}, ����:{ownedAmount}");
            if (ownedAmount < req.Amount)
            {
                Debug.Log($"[���ۺҰ�] ��� ����: {req.ResourceKey} �ʿ�: {req.Amount}, ����: {ownedAmount}");
                hasAll = false;
            }
        }
        if (!hasAll)
        {
            return;
        }

        // ���� ���ɽ� �ݹ� ����
        onSelectCallback?.Invoke();
    }
}
