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
    private GameManager gameManager;
    private UIManager uiManager;

    private enum TabType { Weapon, Gem, Resource }
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

        SwitchTab(TabType.Weapon);
    }

    public void SetWeaponSlotCallback(Action<ItemInstance> callback)
    {
        weaponSlotCallback = callback;
        // 슬롯이 이미 있을 수 있으니 콜백 재연결 위해 Refresh
        if (curTab == TabType.Weapon)
            RefreshSlots();
    }

    private void SwitchTab(TabType tab)
    {
        curTab = tab;
        equipRoot.gameObject.SetActive(tab == TabType.Weapon);
        gemRoot.gameObject.SetActive(tab == TabType.Gem);
        resourceRoot.gameObject.SetActive(tab == TabType.Resource);
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
            foreach (var item in gameManager.Inventory.WeaponList)
                CreateSlot(equipRoot, item, weaponSlotCallback);
        }
        else if (curTab == TabType.Gem)
        {
            foreach (var item in gameManager.Inventory.GemList)
                CreateSlot(gemRoot, item, null);
        }
        else if (curTab == TabType.Resource)
        {
            foreach (var item in gameManager.Inventory.ResourceList)
                CreateSlot(resourceRoot, item, null);
        }
    }

    private void ClearSlots(Transform root)
    {
        foreach (Transform t in root)
            Destroy(t.gameObject);
    }

    private void CreateSlot(Transform parent, ItemInstance item, Action<ItemInstance> callback)
    {
        var go = Instantiate(slotPrefab, parent);
        var slot = go.GetComponent<ForgeInventorySlot>();
        // 반드시 IconLoader로 아이콘 세팅, count 및 클릭 이벤트까지 한 번에
        slot.Init(item, callback);
    }
}
