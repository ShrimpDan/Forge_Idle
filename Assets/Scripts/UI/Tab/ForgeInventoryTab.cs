using UnityEngine;
using UnityEngine.UI;
using System;

public class ForgeInventoryTab : MonoBehaviour
{
    [Header("Tab 버튼")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button gemButton;
    [SerializeField] private Button resourceButton;

    [Header("슬롯 루트")]
    [SerializeField] private Transform equipRoot;
    [SerializeField] private Transform gemRoot;
    [SerializeField] private Transform resourceRoot;

    [Header("슬롯 프리팹")]
    [SerializeField] private GameObject slotPrefab;

    private Action<ItemInstance> weaponSlotCallback;
    private Action<ItemInstance> gemSlotCallback;
    private Action<ItemInstance> resourceSlotCallback;
    private Action<AssistantInstance> assistantSlotCallback;

    private GameManager gameManager;
    private UIManager uIManager;

    private enum TabType { Weapon, Gem, Resource, Trainee }
    private TabType curTab = TabType.Weapon;

    public void Init(GameManager gameManager, UIManager uIManager)
    {
        this.gameManager = gameManager;
        this.uIManager = uIManager;

        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => SwitchTab(TabType.Weapon));
        gemButton.onClick.RemoveAllListeners();
        gemButton.onClick.AddListener(() => SwitchTab(TabType.Gem));
        resourceButton.onClick.RemoveAllListeners();
        resourceButton.onClick.AddListener(() => SwitchTab(TabType.Resource));


        SwitchTab(TabType.Weapon);
    }

    public void SetSlotCallbacks(Action<ItemInstance> weapon, Action<ItemInstance> gem, Action<ItemInstance> resource)
    {
        SetSlotCallbacks(weapon, gem, resource, null);
    }
    public void SetSlotCallbacks(Action<ItemInstance> weapon, Action<ItemInstance> gem, Action<ItemInstance> resource, Action<AssistantInstance> assistant)
    {
        weaponSlotCallback = weapon;
        gemSlotCallback = gem;
        resourceSlotCallback = resource;
        assistantSlotCallback = assistant;
        RefreshSlots();
    }

    private void SwitchTab(TabType tab)
    {
        curTab = tab;
        if (equipRoot) equipRoot.gameObject.SetActive(tab == TabType.Weapon);
        if (gemRoot) gemRoot.gameObject.SetActive(tab == TabType.Gem);
        if (resourceRoot) resourceRoot.gameObject.SetActive(tab == TabType.Resource);
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        ClearSlots(equipRoot);
        ClearSlots(gemRoot);
        ClearSlots(resourceRoot);

        if (gameManager == null) return;

        if (curTab == TabType.Weapon)
        {
            foreach (var item in gameManager.Inventory?.WeaponList ?? new System.Collections.Generic.List<ItemInstance>())
                CreateSlot(equipRoot, item, weaponSlotCallback);
        }
        else if (curTab == TabType.Gem)
        {
            foreach (var item in gameManager.Inventory?.GemList ?? new System.Collections.Generic.List<ItemInstance>())
                CreateSlot(gemRoot, item, gemSlotCallback);
        }
        else if (curTab == TabType.Resource)
        {
            foreach (var item in gameManager.Inventory?.ResourceList ?? new System.Collections.Generic.List<ItemInstance>())
                CreateSlot(resourceRoot, item, resourceSlotCallback);
        }
    }

    private void ClearSlots(Transform root)
    {
        if (root == null) return;
        foreach (Transform t in root)
            Destroy(t.gameObject);
    }

    private void CreateSlot(Transform parent, ItemInstance item, Action<ItemInstance> callback)
    {
        if (parent == null || slotPrefab == null) return;
        var go = Instantiate(slotPrefab, parent);
        var slot = go.GetComponent<ForgeInventorySlot>();
 
        slot.Init(item, i =>
        {
            callback?.Invoke(i);
            if (uIManager != null)
                uIManager.CloseUI(UIName.Forge_Inventory_Popup);
        });
    }
}
