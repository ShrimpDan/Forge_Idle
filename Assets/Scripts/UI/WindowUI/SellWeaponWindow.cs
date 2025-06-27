using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SellWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Image selectedWeaponIcon;
    [SerializeField] private Transform slotContentRoot;        // ScrollView�� Content�� ����
    [SerializeField] private GameObject forgeItemSlotPrefab;   // ���� Forge_ItemSlot ������ ����

    private ItemInstance selectedWeapon;
    private List<GameObject> spawnedSlots = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SellWeaponWindow));
    }

    public override void Open()
    {
        base.Open();
        selectedWeapon = null;
        if (selectedWeaponIcon != null)
        {
            selectedWeaponIcon.sprite = null;
            selectedWeaponIcon.enabled = false;
        }
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        // ���� ���� ����
        foreach (var go in spawnedSlots)
            Destroy(go);
        spawnedSlots.Clear();

        if (gameManager == null || gameManager.Inventory == null) return;

        foreach (var weapon in gameManager.Inventory.WeaponList)
        {
            GameObject slotObj = Instantiate(forgeItemSlotPrefab, slotContentRoot);
            var slot = slotObj.GetComponent<Forge_ItemSlot>();
            slot.Init(weapon, OnWeaponSlotClicked);
            spawnedSlots.Add(slotObj);
        }
    }

    private void OnWeaponSlotClicked(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (selectedWeaponIcon != null && weapon?.Data != null)
        {
            selectedWeaponIcon.sprite = IconLoader.GetIcon(weapon.Data.IconPath);
            selectedWeaponIcon.enabled = true;
        }
    }

    public override void Close()
    {
        base.Close();
    }
}
