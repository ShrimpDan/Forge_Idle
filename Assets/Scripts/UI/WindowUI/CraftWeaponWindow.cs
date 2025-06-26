using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform inputWeaponSlots;
    [SerializeField] private GameObject weaponSlotBtnPrefab;

    private List<Button> slotButtons = new List<Button>();
    private List<Image> slotIcons = new List<Image>();
    private int slotCount = 6;
    private int selectedSlotIndex = -1;
    private UIManager uiManager;
    private GameManager gameManager;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.uiManager = uiManager;
        this.gameManager = gameManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(Close);

        foreach (Transform child in inputWeaponSlots)
            Destroy(child.gameObject);
        slotButtons.Clear();
        slotIcons.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);
            Button btn = go.GetComponent<Button>();
            Image icon = go.GetComponent<Image>();
            int idx = i;
            btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
            slotButtons.Add(btn);
            slotIcons.Add(icon);
        }
    }

    private void OnClickInputWeaponSlot(int index)
    {
        selectedSlotIndex = index;
        var popup = uiManager.OpenUI<Forge_Recipe_Popup>(UIName.Forge_Recipe_Popup);
        popup.Init(gameManager.TestDataManager, uiManager);
        popup.SetRecipeSelectCallback(OnRecipeSelected);


        popup.SetForgeAndInventory(gameManager.Forge, gameManager.Inventory);
    }

    // 레시피 선택시 슬롯에 아이콘 넣기
    private void OnRecipeSelected(ItemData itemData, CraftingData craftingData)
    {
        if (itemData == null || craftingData == null) return;
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slotIcons.Count) return;

        // 성공한 경우만 아이콘 넣기
        slotIcons[selectedSlotIndex].sprite = Resources.Load<Sprite>(itemData.IconPath);
    }

    public override void Open()
    {
        base.Open();
        foreach (var icon in slotIcons)
            icon.sprite = null;
        selectedSlotIndex = -1;
    }

    public override void Close()
    {
        base.Close();
    }
}
