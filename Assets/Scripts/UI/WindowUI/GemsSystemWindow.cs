using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Button weaponInputBtn;
    [SerializeField] private Button executeBtn; // 
    [SerializeField] private Image weaponIconImg;
    [SerializeField] private RectTransform gemSlotRoot;
    [SerializeField] private GameObject gemSlotPrefab;

    private ItemInstance selectedWeapon;
    private ItemInstance[] selectedGems = new ItemInstance[3]; 
    private List<Forge_ItemSlot> gemSlots = new List<Forge_ItemSlot>();
    private bool isExecuted = false; 

    private GameManager gameManager;
    private UIManager uIManager;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;

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

        // 기존 슬롯에 있던 보석 복구 (실행 전이라면)
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
            // 실제 보석 인벤토리 수량 1 감소 (0되면 알아서 삭제)
            gameManager.Inventory.UseItem(gem);
        }

        selectedGems[slotIdx] = gem;

        var slot = gemSlots[slotIdx];
        slot.Init(gem, (item) => { OpenGemInventoryPopup(slotIdx); });
    }

    // [실행버튼 클릭] 실제 강화 로직 적용 자리
    private void OnExecute()
    {
        if (selectedWeapon == null)
        {
            Debug.LogWarning("[GemsSystem] 무기를 먼저 선택하세요!");
            return;
        }

        // 여기에 강화 수치 적용 로직 삽입

        Debug.Log("[GemsSystem] Execute 완료! (강화 적용 로직 자리)");

        isExecuted = true; // 실행 완료 flag (Exit 때 복구 방지)
    }

    // [나가기 버튼] 보석 반환 처리
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
        isExecuted = false; // 리셋
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
