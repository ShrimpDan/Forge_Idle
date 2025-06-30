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
    private ItemInstance[] selectedGems = new ItemInstance[3]; // 3개의 보석 슬롯
    private List<Forge_ItemSlot> gemSlots = new List<Forge_ItemSlot>();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.GemsSystemWindow));

        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);

        ResetAll();
    }

    void OpenWeaponInventoryPopup()
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        // 무기만 선택 가능
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
        // 보석 슬롯 새로 생성
        ResetGemSlots();
    }

    private void ResetGemSlots()
    {
        // 기존 슬롯 삭제
        foreach (Transform t in gemSlotRoot)
            Destroy(t.gameObject);
        gemSlots.Clear();

        if (selectedWeapon != null)
        {
            for (int i = 0; i < 3; i++)
            {
                int slotIdx = i; // 클로저 이슈 방지
                var go = Instantiate(gemSlotPrefab, gemSlotRoot);
                var slot = go.GetComponent<Forge_ItemSlot>();
                gemSlots.Add(slot);

                slot.Init(selectedGems[i], (item) => {
                    // 보석 인벤토리 팝업 열기 (해당 슬롯에만)
                    OpenGemInventoryPopup(slotIdx);
                });
            }
        }
    }

    private void OpenGemInventoryPopup(int slotIdx)
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        // 보석만 선택 콜백
        popup.SetWeaponSelectCallback((gem) => OnGemSelected(slotIdx, gem));
        // 만약 Tab 자동 Gem 전환이 안 된다면 팝업에서 GemTab 강제 전환 필요 (수정점)
    }

    private void OnGemSelected(int slotIdx, ItemInstance gem)
    {
        if (slotIdx < 0 || slotIdx >= 3) return;
        selectedGems[slotIdx] = gem;
        // 슬롯 이미지 갱신
        var slot = gemSlots[slotIdx];
        slot.Init(gem, (item) => { OpenGemInventoryPopup(slotIdx); }); // 재세팅
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
