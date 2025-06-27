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

    public void Init(GameManager gameManager, UIManager uiManager)
    {
        this.gameManager = gameManager;
        this.uiManager = uiManager;

        RefreshSlots();
    }

    public void SetWeaponSlotCallback(Action<ItemInstance> callback)
    {
        weaponSlotCallback = callback;
    }

    // ½½·Ô »ý¼º/ÃÊ±âÈ­ ¿¹½Ã 
    public void RefreshSlots()
    {
        foreach (Transform child in equipRoot)
            Destroy(child.gameObject);

        var weaponList = gameManager.Inventory.WeaponList;
        foreach (var item in weaponList)
        {
            var go = GameObject.Instantiate(slotPrefab, equipRoot);
            var btn = go.GetComponent<Button>();
            var icon = go.GetComponent<Image>();
            icon.sprite = IconLoader.GetIcon(item.Data.IconPath);

            btn.onClick.AddListener(() => weaponSlotCallback?.Invoke(item));
        }
    }
}
