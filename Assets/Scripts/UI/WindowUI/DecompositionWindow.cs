using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DecompositionWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    private InventoryManager inventory;
    private InventoryTab inventoryTab;
    private CraftingDataLoader craftingDataLoader;
    private ItemDataLoader itemDataLoader;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform equipRoot;
    [SerializeField] private Transform decompositionRoot;
    [SerializeField] private Button decompositionBtn;
    [SerializeField] private Button exitBtn;

    private Queue<GameObject> pooledSlots = new Queue<GameObject>();
    private List<GameObject> activeSlots = new List<GameObject>();

    private Dictionary<string, float> resultDict = new Dictionary<string, float>();
    private List<ItemInstance> clickedWeapons = new List<ItemInstance>();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        inventory = gameManager.Inventory;
        itemDataLoader = gameManager.DataManager.ItemLoader;
        craftingDataLoader = gameManager.DataManager.CraftingLoader;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.DecompositionWindow));

        decompositionBtn.onClick.RemoveAllListeners();
        decompositionBtn.onClick.AddListener(ClickDecompositionBtn);
    }

    public void SetUI(InventoryTab inventoryTab)
    {
        this.inventoryTab = inventoryTab;
    }

    public override void Open()
    {
        base.Open();

        RefreshSlots();
    }

    public override void Close()
    {
        base.Close();
    }

    public void RefreshSlots()
    {
        foreach (var slot in activeSlots)
        {
            if (slot == null) continue;

            slot.SetActive(false);
            pooledSlots.Enqueue(slot);
        }
        activeSlots.Clear();

        CreateSlots(inventory.WeaponList, equipRoot);
    }

    private void CreateSlots(List<ItemInstance> itemList, Transform parent)
    {
        foreach (var item in itemList)
        {
            if (item.IsEquipped) continue;

            GameObject slotObj = GetSlotFromPool();
            slotObj.transform.SetParent(parent, false);
            slotObj.SetActive(true);

            var slot = slotObj.GetComponent<DecompositionSlot>();
            slot.Init(this, item);

            activeSlots.Add(slotObj);
        }
    }

    private GameObject GetSlotFromPool()
    {
        if (pooledSlots.Count > 0)
            return pooledSlots.Dequeue();

        return Instantiate(slotPrefab);
    }

    public void ClickWeaponSlot(ItemInstance item, bool isSelected)
    {
        if (isSelected)
        {
            clickedWeapons.Add(item);

            CraftingData craftingData = craftingDataLoader.GetDataByKey(item.ItemKey);

            foreach (var resource in craftingData.RequiredResources)
            {
                if (resultDict.ContainsKey(resource.ResourceKey))
                    resultDict[resource.ResourceKey] += resource.Amount;
                else
                    resultDict.Add(resource.ResourceKey, resource.Amount);
            }
        }
        else
        {
            if(clickedWeapons.Contains(item))
                clickedWeapons.Remove(item);

            CraftingData craftingData = craftingDataLoader.GetDataByKey(item.ItemKey);

            foreach (var resource in craftingData.RequiredResources)
            {
                if (resultDict.ContainsKey(resource.ResourceKey))
                {
                    resultDict[resource.ResourceKey] -= resource.Amount;

                    if (resultDict[resource.ResourceKey] <= 0)
                        resultDict.Remove(resource.ResourceKey);
                }
            }
        }

        UpdateResultRoot();
    }

    private void UpdateResultRoot()
    {
        foreach (Transform child in decompositionRoot)
        {
            child.gameObject.SetActive(false);
            pooledSlots.Enqueue(child.gameObject);
        }

        foreach (var resourceKey in resultDict.Keys)
        {
            var itemData = itemDataLoader.GetItemByKey(resourceKey);

            GameObject slotObj = GetSlotFromPool();
            slotObj.transform.SetParent(decompositionRoot, false);
            slotObj.SetActive(true);

            int minCount = Mathf.FloorToInt(resultDict[resourceKey] * 0.3f);
            int maxCount = Mathf.CeilToInt(resultDict[resourceKey] * 0.5f);

            var slot = slotObj.GetComponent<DecompositionSlot>();
            slot.Init(itemData, minCount, maxCount);

            activeSlots.Add(slotObj);
        }
    }

    private void ClickDecompositionBtn()
    {
        if (resultDict.Count == 0)
        {
            var lackPopup = uIManager.OpenUI<LackPopup>(UIName.LackPopup);
            lackPopup.ShowCustom("아이템을 선택해주세요.");
            return;
        }

        var popup = uIManager.OpenUI<DecompositionPopup>(UIName.DecompositionPopup);
        popup.SetUI(this);
    }

    public void DecompositionWeapons()
    {
        Dictionary<ItemData, int> rewardDict = new Dictionary<ItemData, int>();

        foreach (var pair in resultDict)
        {
            string resourceKey = pair.Key;
            float totalAmount = pair.Value;

            int minCount = Mathf.FloorToInt(totalAmount * 0.3f);
            int maxCount = Mathf.CeilToInt(totalAmount * 0.5f);

            if (minCount > maxCount)
            {
                minCount = maxCount;
            }

            int randomAmount = (int)totalAmount <= 0
                ? Random.Range(0, 2)
                : Random.Range(minCount, maxCount + 1);

            if (randomAmount > 0)
            {
                ItemData itemData = itemDataLoader.GetItemByKey(resourceKey);
                if (itemData != null)
                {
                    if (rewardDict.ContainsKey(itemData))
                    {
                        rewardDict[itemData] += randomAmount;
                    }
                    else
                    {
                        rewardDict.Add(itemData, randomAmount);
                    }
                }
            }
        }

        var rewardPopup = uIManager.OpenUI<RewardPopup>(UIName.RewardPopup);
        rewardPopup.Show(rewardDict);

        foreach (var itemData in rewardDict.Keys)
        {
            inventory.AddItem(itemData, rewardDict[itemData]);
        }

        foreach (var weapon in clickedWeapons)
        {
            inventory.RemoveItem(weapon);
        }

        clickedWeapons.Clear();
        resultDict.Clear();
        RefreshSlots();
        UpdateResultRoot();

        inventoryTab.RefreshSlots();
    }
}
