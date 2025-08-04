using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecompositionWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    private InventoryManager inventory;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform equipRoot;
    [SerializeField] private Transform decompositionRoot;
    [SerializeField] private Button decompositionBtn;
    [SerializeField] private Button exitBtn;

    private Queue<GameObject> pooledSlots = new Queue<GameObject>();
    private List<GameObject> activeSlots = new List<GameObject>();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        inventory = gameManager.Inventory;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.DecompositionWindow));
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
            if (item.Quantity <= 0) continue;

            GameObject slotObj = GetSlotFromPool();
            slotObj.transform.SetParent(parent, false);
            slotObj.SetActive(true);

            var slot = slotObj.GetComponent<InventorySlot>();
            slot.Init(item);

            activeSlots.Add(slotObj);
        }
    }

    private GameObject GetSlotFromPool()
    {
        if (pooledSlots.Count > 0)
            return pooledSlots.Dequeue();

        return Instantiate(slotPrefab);
    }
}
