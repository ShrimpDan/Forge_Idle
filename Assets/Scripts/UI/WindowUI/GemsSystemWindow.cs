using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button weaponInputBtn;
    [SerializeField] private Image weaponIconImg;
    [SerializeField] private RectTransform gemSlotRoot;
    [SerializeField] private GameObject gemSlotPrefab;

    private ItemInstance selectedWeapon;
    private ItemInstance[] selectedGems = new ItemInstance[3]; // 3개의 보석 슬롯 (실제 장착 상태)
    private List<Forge_ItemSlot> gemSlots = new List<Forge_ItemSlot>();

    private GameManager gameManager;
    private UIManager uIManager;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.GemsSystemWindow));
        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);

        ResetAll();
    }

    void OpenWeaponInventoryPopup()
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetWeaponSelectCallback(OnWeaponSelected);
    }

    private void OnWeaponSelected(ItemInstance weapon)
    {
        selectedWeapon = weapon;
        if (weaponIconImg != null)
        {
            weaponIconImg.sprite = IconLoader.GetIcon(weapon.Data.IconPath);
            weaponIconImg.enabled = true;
        }
        ResetGemSlots();
    }

    private void ResetGemSlots()
    {
        foreach (Transform t in gemSlotRoot)
            Destroy(t.gameObject);
        gemSlots.Clear();

        if (selectedWeapon != null)
        {
            for (int i = 0; i < 3; i++)
            {
                int slotIdx = i;
                var go = Instantiate(gemSlotPrefab, gemSlotRoot);
                var slot = go.GetComponent<Forge_ItemSlot>();
                gemSlots.Add(slot);

                slot.Init(selectedGems[i], (item) =>
                {
                    OpenGemInventoryPopup(slotIdx);
                });
            }
        }
    }

    private void OpenGemInventoryPopup(int slotIdx)
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetGemSelectCallback((gem) => OnGemSelected(slotIdx, gem));
    }

    private void OnGemSelected(int slotIdx, ItemInstance gem)
    {
        if (slotIdx < 0 || slotIdx >= 3) return;

        // 기존 슬롯에 있던 보석 복구
        var oldGem = selectedGems[slotIdx];
        if (oldGem != null)
        {
            oldGem.Quantity += 1;
            if (!gameManager.Inventory.GemList.Contains(oldGem))
            {
                gameManager.Inventory.GemList.Add(oldGem);
            }
        }

  
        if (gem != null)
        {
            // 실제 보석 인벤토리 수량 1 감소 (0되면 알아서 삭제)
            gameManager.Inventory.UseItem(gem);
        }

        selectedGems[slotIdx] = gem;


        var slot = gemSlots[slotIdx];
        slot.Init(gem, (item) => { OpenGemInventoryPopup(slotIdx); });

    }

    private void ResetAll()
    {
        selectedWeapon = null;
        for (int i = 0; i < selectedGems.Length; i++)
            selectedGems[i] = null;

        if (weaponIconImg != null)
        {
            weaponIconImg.sprite = null;
            weaponIconImg.enabled = false;
        }
        ResetGemSlots();
    }

    public override void Open()
    {
        base.Open();
        ResetAll();

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.GemsSystemWindow));
        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);
    }

    public override void Close()
    {
        base.Close();
    }
}
