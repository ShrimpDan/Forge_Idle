using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTab : BaseTab
{
    private InventoryManager inventory;

    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("To Create InvenSlot Root")]
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform EquipRoot;
    [SerializeField] Transform GemRoot;
    [SerializeField] Transform ResourceRoot;

    [Header("Equipped Weapon Slots")]
    [SerializeField] InvenEquippedSlot[] invenEquippedSlots;

    private Queue<GameObject> pooledSlots = new Queue<GameObject>();
    private List<GameObject> activeSlots = new List<GameObject>();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        SwitchTab(0);

        inventory = gameManager.Inventory;
        RefreshSlots();
    }

    public override void OpenTab()
    {
        base.OpenTab();

        inventory.OnItemAdded += Refresh;
        inventory.OnItemEquipped += OnItemEquipped;
        inventory.OnItemUnEquipped += OnItemUnEquipped;
        RefreshSlots();
    }

    public override void CloseTab()
    {
        base.CloseTab();

        inventory.OnItemAdded -= Refresh;
        inventory.OnItemEquipped -= OnItemEquipped;
        inventory.OnItemUnEquipped -= OnItemUnEquipped;
    }

    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;

            tabPanels[i].SetActive(isSelected);

            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }
    }

    public void RefreshSlots()
    {
        foreach (var slot in activeSlots)
        {
            slot.SetActive(false);
            pooledSlots.Enqueue(slot);
        }
        activeSlots.Clear();

        CreateSlots(inventory.WeaponList, EquipRoot);
        CreateSlots(inventory.GemList, GemRoot);
        CreateSlots(inventory.ResourceList, ResourceRoot);
    }

    private void CreateSlots(List<ItemInstance> itemList, Transform parent)
    {
        foreach (var item in itemList)
        {
            if (item.Quantity <= 0) continue;

            GameObject slotObj = GetSlotFromPool();
            slotObj.transform.SetParent(parent, false);
            slotObj.SetActive(true);

            var slot = slotObj.GetComponent<InventorySlot>();
            slot.Init(item);

            activeSlots.Add(slotObj);
        }
    }

    public void Refresh()
    {
        RefreshSlots();
    }

    private GameObject GetSlotFromPool()
    {
        if (pooledSlots.Count > 0)
            return pooledSlots.Dequeue();

        return Instantiate(slotPrefab);
    }

    private void OnItemEquipped(int slotIndex, ItemInstance item)
    {
        UpdateSlot(item, true);
        invenEquippedSlots[slotIndex].Init(uIManager, item);
    }

    private void OnItemUnEquipped(int slotIndex, ItemInstance item)
    {
        UpdateSlot(item, false);
        invenEquippedSlots[slotIndex].UnEquipItem();
    }

    private void UpdateSlot(ItemInstance item, bool isEquipped)
    {
        foreach (var obj in activeSlots)
        {
            InventorySlot slot = obj.GetComponent<InventorySlot>();
            if (slot.SlotItem == item)
            {
                slot.SetEquipped(isEquipped);
                break;
            }
        }
    }
}
