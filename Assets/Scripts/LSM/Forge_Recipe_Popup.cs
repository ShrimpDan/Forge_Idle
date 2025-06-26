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

    private DataManger dataManager;
    private Action<ItemData, CraftingData> onRecipeSelect;
    private Forge forge;
    private Jang.InventoryManager inventory;

    public void Init(DataManger dataManager, UIManager uiManager)
    {
        this.dataManager = dataManager;
        this.uIManager = uiManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.Forge_Recipe_Popup));
        PopulateRecipeList();
    }

    public void SetRecipeSelectCallback(Action<ItemData, CraftingData> callback)
    {
        onRecipeSelect = callback;
    }

    // 추가: Forge와 Inventory를 세팅할 수 있도록
    public void SetForgeAndInventory(Forge forge, Jang.InventoryManager inventory)
    {
        this.forge = forge;
        this.inventory = inventory;
    }

    private void PopulateRecipeList()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        var craftingList = dataManager?.CraftingLoader?.CraftingList;
        var itemLoader = dataManager?.ItemLoader;

        if (craftingList == null || itemLoader == null)
        {
            Debug.LogError("[Forge_Recipe_Popup] 데이터 없음");
            return;
        }

        foreach (var data in craftingList)
        {
            var go = Instantiate(recipeSlotPrefab, contentRoot);
            var slot = go.GetComponent<RecipeSlot>();
            if (slot != null)
            {
                slot.Setup(data, itemLoader);
                // 인벤토리/포지 정보를 넘겨주고, 선택시 이벤트 연결
                slot.SetSelectContext(itemLoader, forge, inventory, () =>
                {
                    onRecipeSelect?.Invoke(itemLoader.GetItemByKey(data.ItemKey), data);
                    uIManager.CloseUI(UIName.Forge_Recipe_Popup);
                });
            }
        }
    }
}
