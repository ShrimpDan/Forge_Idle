using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ForgeInventoryTab : MonoBehaviour
{
    [Header("Tab ¹öÆ°")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button gemButton;
    [SerializeField] private Button resourceButton;

    [Header("½½·Ô ·çÆ®")]
    [SerializeField] private Transform equipRoot;
    [SerializeField] private Transform gemRoot;
    [SerializeField] private Transform resourceRoot;

    [Header("½½·Ô ÇÁ¸®ÆÕ")]
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
        // ½½·Ô Å¬¸®¾î
        foreach (Transform t in equipRoot) Destroy(t.gameObject);
        foreach (Transform t in gemRoot) Destroy(t.gameObject);
        foreach (Transform t in resourceRoot) Destroy(t.gameObject);

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

    private void CreateSlot(Transform parent, ItemInstance item, Action<ItemInstance> callback)
    {
        var go = Instantiate(slotPrefab, parent);
        var btn = go.GetComponent<Button>();
        var icon = go.GetComponent<Image>();
        if (icon != null && item?.Data != null)
            icon.sprite = IconLoader.GetIcon(item.Data.IconPath);

        btn.onClick.RemoveAllListeners();
        if (callback != null)
        {
            btn.onClick.AddListener(() => callback(item));
        }
    }
}
