using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button weaponInputBtn;
    [SerializeField] private Button executeBtn;
    [SerializeField] private Image weaponIconImg;
    [SerializeField] private RectTransform gemSlotRoot;
    [SerializeField] private GameObject gemSlotPrefab;

    private ItemInstance selectedWeapon;
    private ItemInstance[] selectedGems = new ItemInstance[3];
    private List<Forge_ItemSlot> gemSlots = new List<Forge_ItemSlot>();
    private bool isExecuted = false;

    private DataManager dataManager;  // ★ 추가: 데이터 매니저 직접 참조

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;
        this.dataManager = gameManager?.DataManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(OnExit);

        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);

        if (executeBtn != null)
        {
            executeBtn.onClick.RemoveAllListeners();
            executeBtn.onClick.AddListener(OnExecute);
        }

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
        if (weapon != null && weapon.Data == null && dataManager != null)
        {
            weapon.Data = dataManager.ItemLoader.GetItemByKey(weapon.ItemKey);
        }
        if (weaponIconImg != null && selectedWeapon?.Data != null)
        {
            weaponIconImg.sprite = IconLoader.GetIconByPath(selectedWeapon.Data.IconPath);
            weaponIconImg.enabled = true;
        }
        else if (weaponIconImg != null)
        {
            weaponIconImg.sprite = null;
            weaponIconImg.enabled = false;
        }

        // 장착된 젬 슬롯에 표시
        if (selectedWeapon != null && selectedWeapon.GemSockets != null)
        {
            for (int i = 0; i < selectedGems.Length; i++)
            {
                selectedGems[i] = selectedWeapon.GemSockets.Count > i ? selectedWeapon.GemSockets[i] : null;
            }
        }
        else
        {
            for (int i = 0; i < selectedGems.Length; i++)
                selectedGems[i] = null;
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
        if (!isExecuted && oldGem != null)
        {
            oldGem.Quantity += 1;
            if (!gameManager.Inventory.GemList.Contains(oldGem))
            {
                gameManager.Inventory.GemList.Add(oldGem);
            }
        }

        if (gem != null)
        {
            // 혹시 Data가 비어있다면 DataLoader에서 보충
            if (gem.Data == null && dataManager != null)
                gem.Data = dataManager.ItemLoader.GetItemByKey(gem.ItemKey);

            gameManager.Inventory.UseItem(gem);
        }

        selectedGems[slotIdx] = gem;

        var slot = gemSlots[slotIdx];
        slot.Init(gem, (item) => { OpenGemInventoryPopup(slotIdx); });
    }

    // 실제 강화 로직 적용 자리
    private void OnExecute()
    {
        if(selectedWeapon == null)
    {
            Debug.LogWarning("[GemsSystem] 무기를 먼저 선택하세요!");
            return;
        }

        // 선택된 젬을 무기에 연결
        for (int i = 0; i < 3; i++)
        {
            selectedWeapon.GemSockets[i] = selectedGems[i];
        }

        Debug.Log("[GemsSystem] Execute 완료! (젬 소켓 연결 완료)");
        isExecuted = true;
    }

    // 보석 반환 처리
    private void OnExit()
    {
        if (!isExecuted)
        {
            for (int i = 0; i < selectedGems.Length; i++)
            {
                var gem = selectedGems[i];
                if (gem != null)
                {
                    gem.Quantity += 1;
                    if (!gameManager.Inventory.GemList.Contains(gem))
                        gameManager.Inventory.GemList.Add(gem);
                    selectedGems[i] = null;
                }
            }
            Debug.Log("[GemsSystem] 창 종료: 사용된 보석을 인벤토리에 복구함");
        }
        uIManager.CloseUI(UIName.GemsSystemWindow);
        ResetAll();
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
        isExecuted = false;
        ResetGemSlots();
    }

    public override void Open()
    {
        base.Open();
        ResetAll();

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(OnExit);

        weaponInputBtn.onClick.RemoveAllListeners();
        weaponInputBtn.onClick.AddListener(OpenWeaponInventoryPopup);

        if (executeBtn != null)
        {
            executeBtn.onClick.RemoveAllListeners();
            executeBtn.onClick.AddListener(OnExecute);
        }
    }

    public override void Close()
    {
        base.Close();
    }
}
