using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class Gem_Weapon_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [Header("UI")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Transform weaponListRoot;
    [SerializeField] private GameObject weaponSlotPrefab;
    [SerializeField] private ScrollRect scrollRect;

    private Action<ItemInstance> weaponSelectCallback;
    private List<GameObject> slotInstances = new();

    private DataManager dataManager;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.dataManager = gameManager.DataManager;

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.Gem_Weapon_Popup));
    }

    public void SetWeaponSelectCallback(Action<ItemInstance> callback)
    {
        weaponSelectCallback = callback;
        RefreshWeaponList();
    }

    private void RefreshWeaponList()
    {
        if (weaponListRoot == null || weaponSlotPrefab == null)
            return;

        foreach (var go in slotInstances)
            Destroy(go);
        slotInstances.Clear();

        if (gameManager == null || gameManager.Inventory == null)
            return;

        var weaponList = gameManager.Inventory.WeaponList ?? new List<ItemInstance>();
        foreach (var weapon in weaponList)
        {
            ItemData itemData = dataManager.ItemLoader.GetItemByKey(weapon.ItemKey);
            if (itemData == null) continue;

            if (weapon.GemSockets == null)
                weapon.GemSockets = new List<ItemInstance> { null, null, null, null };

            var go = Instantiate(weaponSlotPrefab, weaponListRoot);
            slotInstances.Add(go);

            var slot = go.GetComponent<GemWeaponSelectSlot>();
            if (slot != null)
            {
                slot.Init(weapon, (w) =>
                {
                    weaponSelectCallback?.Invoke(w);
                    uIManager.CloseUI(UIName.Gem_Weapon_Popup);
                });
            }
            else
            {
                var icon = go.transform.Find("IconBG/Icon")?.GetComponent<Image>();
                var nameText = go.transform.Find("NameText")?.GetComponent<TMP_Text>();

                if (icon != null)
                {
                    // UpgradeWeaponWindow와 동일! (ItemType, ItemKey)
                    icon.sprite = IconLoader.GetIcon(itemData.ItemType, itemData.ItemKey);
                    icon.enabled = true;
                }
                if (nameText != null)
                    nameText.text = itemData.Name;

                var gemSlotsRoot = go.transform.Find("GemSlots");
                if (gemSlotsRoot != null && weapon.GemSockets != null)
                {
                    for (int i = 0; i < gemSlotsRoot.childCount; i++)
                    {
                        var gemSlot = gemSlotsRoot.GetChild(i);
                        var gemIcon = gemSlot.GetComponentInChildren<Image>();
                        var gem = (weapon.GemSockets.Count > i) ? weapon.GemSockets[i] : null;
                        if (gemIcon != null)
                        {
                            if (gem != null && gem.Data != null)
                            {
                                // UpgradeWeaponWindow와 똑같이, 젬도 ItemType, ItemKey로!
                                gemIcon.sprite = IconLoader.GetIcon(gem.Data.ItemType, gem.Data.ItemKey);
                                gemIcon.enabled = true;
                            }
                            else
                            {
                                gemIcon.sprite = null;
                                gemIcon.enabled = false;
                            }
                        }
                    }
                }

                var btn = go.GetComponent<Button>();
                if (btn == null) btn = go.AddComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    weaponSelectCallback?.Invoke(weapon);
                    uIManager.CloseUI(UIName.Gem_Weapon_Popup);
                });
            }
        }

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    public override void Open()
    {
        base.Open();
        RefreshWeaponList();
    }
}
