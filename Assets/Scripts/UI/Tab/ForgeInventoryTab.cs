using UnityEngine;
using UnityEngine.UI;
using System;

public class ForgeInventoryTab : MonoBehaviour
{
    [Header("Tab 버튼")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button gemButton;
    [SerializeField] private Button resourceButton;
    [SerializeField] private Button traineeButton; 

    [Header("슬롯 루트")]
    [SerializeField] private Transform equipRoot;
    [SerializeField] private Transform gemRoot;
    [SerializeField] private Transform resourceRoot;
    [SerializeField] private Transform traineeRoot;

    [Header("슬롯 프리팹")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject traineeSlotPrefab;

    private Action<ItemInstance> weaponSlotCallback;
    private Action<ItemInstance> gemSlotCallback;
    private Action<ItemInstance> resourceSlotCallback;
    private Action<TraineeData> traineeSlotCallback;

    private GameManager gameManager;
    private UIManager uiManager;

    private enum TabType { Weapon, Gem, Resource, Trainee }
    private TabType curTab = TabType.Weapon;

    public void Init(GameManager gameManager, UIManager uiManager)
    {
        this.gameManager = gameManager;
        this.uiManager = uiManager;

        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => SwitchTab(TabType.Weapon));
        gemButton.onClick.RemoveAllListeners();
        gemButton.onClick.AddListener(() => SwitchTab(TabType.Gem));
        resourceButton.onClick.RemoveAllListeners();
        resourceButton.onClick.AddListener(() => SwitchTab(TabType.Resource));

        if (traineeButton != null && traineeRoot != null)
        {
            traineeButton.onClick.RemoveAllListeners();
            traineeButton.onClick.AddListener(() => SwitchTab(TabType.Trainee));
        }

        SwitchTab(TabType.Weapon);
    }

    public void SetSlotCallbacks(Action<ItemInstance> weapon, Action<ItemInstance> gem, Action<ItemInstance> resource)
    {
        SetSlotCallbacks(weapon, gem, resource, null);
    }
    public void SetSlotCallbacks(Action<ItemInstance> weapon, Action<ItemInstance> gem, Action<ItemInstance> resource, Action<TraineeData> trainee)
    {
        weaponSlotCallback = weapon;
        gemSlotCallback = gem;
        resourceSlotCallback = resource;
        traineeSlotCallback = trainee;
        RefreshSlots();
    }

    private void SwitchTab(TabType tab)
    {
        curTab = tab;
        if (equipRoot) equipRoot.gameObject.SetActive(tab == TabType.Weapon);
        if (gemRoot) gemRoot.gameObject.SetActive(tab == TabType.Gem);
        if (resourceRoot) resourceRoot.gameObject.SetActive(tab == TabType.Resource);
        if (traineeRoot) traineeRoot.gameObject.SetActive(tab == TabType.Trainee);
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        ClearSlots(equipRoot);
        ClearSlots(gemRoot);
        ClearSlots(resourceRoot);
        if (traineeRoot != null) ClearSlots(traineeRoot);

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
        else if (curTab == TabType.Trainee && traineeRoot != null && gameManager.TraineeInventory != null)
        {
            foreach (var trainee in gameManager.TraineeInventory.GetAll())
                CreateTraineeSlot(traineeRoot, trainee, traineeSlotCallback);
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
            if (uiManager != null)
                uiManager.CloseUI(UIName.Forge_Inventory_Popup);
        });
    }

    private void CreateTraineeSlot(Transform parent, TraineeData trainee, Action<TraineeData> callback)
    {
        if (parent == null || traineeSlotPrefab == null) return;
        var go = Instantiate(traineeSlotPrefab, parent);
        var slot = go.GetComponent<AssistantSlot>();
        slot.Init(trainee, t =>
        {
            callback?.Invoke(t);
            if (uiManager != null)
                uiManager.CloseUI(UIName.Forge_Inventory_Popup);
        });
    }
}
