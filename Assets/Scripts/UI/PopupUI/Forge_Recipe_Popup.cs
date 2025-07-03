using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Forge_Recipe_Popup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject recipeSlotPrefab;

    private DataManager dataManager;
    private Action<ItemData, CraftingData> onRecipeSelect;
    private Forge forge;
    private InventoryManager inventory;

    // 팝업 오픈 시마다 반드시 호출
    public void Init(DataManager dataManager, UIManager uiManager)
    {
        this.dataManager = dataManager;
        this.uIManager = uiManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.Forge_Recipe_Popup));
    }

    // 제작 선택 콜백
    public void SetRecipeSelectCallback(Action<ItemData, CraftingData> callback)
    {
        onRecipeSelect = callback;
    }

    // 현재 포지/인벤토리 전달 
    public void SetForgeAndInventory(Forge forge, InventoryManager inventory)
    {
        this.forge = forge;
        this.inventory = inventory;
        PopulateRecipeList();
    }

    // 레시피 리스트 생성 
    private void PopulateRecipeList()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        var craftingList = dataManager?.CraftingLoader?.CraftingList;
        var itemLoader = dataManager?.ItemLoader;

        if (craftingList == null || itemLoader == null)
        {
            Debug.LogError("[Forge_Recipe_Popup] 레시피 데이터 또는 아이템 데이터가 없습니다.");
            return;
        }

        foreach (var data in craftingList)
        {
            var go = Instantiate(recipeSlotPrefab, contentRoot);
            var slot = go.GetComponent<RecipeSlot>();
            if (slot != null)
            {
                slot.Setup(data, itemLoader, forge, inventory, () =>
                {
                    if (onRecipeSelect != null)
                    {
                        onRecipeSelect(itemLoader.GetItemByKey(data.ItemKey), data);
                        uIManager.CloseUI(UIName.Forge_Recipe_Popup);
                    }
                });
            }
        }
    }
}
