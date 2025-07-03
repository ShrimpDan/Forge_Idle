using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RefineSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Transform slotRoot;
    [SerializeField] private GameObject refineSlotPrefab; // RefineSlotUI

    private const int slotCount = 5;
    private List<RefineSlotUI> refineSlots = new();
    private List<ItemInstance> selectedMaterials = new();
    private List<ItemInstance> resultItems = new();

    private GameManager gameManager;
    private UIManager uIManager;
    private DataManager dataManager;

    private int baseRefineCost = 1000;
    private int requiredAmount = 5;
    private int resultAmount = 1;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;
        this.dataManager = gameManager?.DataManager;

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.RefineSystemWindow));

        InitSlots();
        ResetUI();
    }

    private void InitSlots()
    {
        foreach (Transform child in slotRoot)
            Destroy(child.gameObject);
        refineSlots.Clear();
        selectedMaterials = new List<ItemInstance>(new ItemInstance[slotCount]);
        resultItems = new List<ItemInstance>(new ItemInstance[slotCount]);

        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(refineSlotPrefab, slotRoot);
            var slot = go.GetComponent<RefineSlotUI>();
            int idx = i;

            // 버튼 이벤트 모두 직접 연결
            slot.inputButton.onClick.RemoveAllListeners();
            slot.inputButton.onClick.AddListener(() => OnClickInputSlot(idx));

            slot.minusBtn.onClick.RemoveAllListeners();
            slot.minusBtn.onClick.AddListener(() => {
                slot.SetAmount(slot.Amount - 1);
                UpdateSlot(idx);
            });

            slot.plusBtn.onClick.RemoveAllListeners();
            slot.plusBtn.onClick.AddListener(() => {
                slot.SetAmount(slot.Amount + 1);
                UpdateSlot(idx);
            });

            slot.executeBtn.onClick.RemoveAllListeners();
            slot.executeBtn.onClick.AddListener(() => OnClickExecute(idx));

            slot.SetAmount(1);
            refineSlots.Add(slot);
        }
    }

    private void OnClickInputSlot(int index)
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetResourceSelectCallback((item) => OnMaterialSelected(index, item));
    }

    private void OnMaterialSelected(int index, ItemInstance item)
    {
        if (item != null && item.Data == null && dataManager != null)
            item.Data = dataManager.ItemLoader.GetItemByKey(item.ItemKey);

        selectedMaterials[index] = item;
        UpdateSlot(index);
    }

    private void UpdateSlot(int index)
    {
        var slot = refineSlots[index];
        var input = selectedMaterials[index];

        slot.inputIcon.sprite = input?.Data != null ? IconLoader.GetIcon(input.Data.IconPath) : null;
        slot.inputIcon.enabled = input?.Data != null;

        var output = GetRefineResult(input);
        resultItems[index] = output;
        slot.outputIcon.sprite = output?.Data != null ? IconLoader.GetIcon(output.Data.IconPath) : null;
        slot.outputIcon.enabled = output?.Data != null;

        int totalCost = baseRefineCost * slot.Amount;
        if (slot.costText != null) slot.costText.text = $"비용: {totalCost:N0}";

        if (slot.amountText != null) slot.amountText.text = slot.Amount.ToString();
    }

    private ItemInstance GetRefineResult(ItemInstance input)
    {
        if (input?.Data == null) return null;
        if (!input.ItemKey.StartsWith("resource_")) return null;

        string coreName = input.ItemKey.Substring("resource_".Length);
        string[] gemTypes = { "ruby", "emerald", "amethyst", "sapphire" };
        string[] metalTypes = { "copper", "bronze", "iron", "silver", "gold", "mithril" };

        string outKey = null;
        if (gemTypes.Contains(coreName))
            outKey = "gem_" + coreName;
        else if (metalTypes.Contains(coreName))
            outKey = "ingot_" + coreName;

        if (outKey == null) return null;
        var outData = dataManager.ItemLoader.GetItemByKey(outKey);
        if (outData == null) return null;
        return new ItemInstance(outData.ItemKey, outData);
    }

    private void OnClickExecute(int index)
    {
        var slot = refineSlots[index];
        var input = selectedMaterials[index];
        var output = resultItems[index];
        int amount = slot.Amount;
        int cost = baseRefineCost * amount;
        if (input == null || output == null) return;
        if (gameManager.Forge.Gold < cost) return;

        int owned = gameManager.Inventory.ResourceList
            .Where(x => x.ItemKey == input.ItemKey)
            .Sum(x => x.Quantity);

        if (owned < requiredAmount * amount) return;

        var reqList = new List<(string resourceKey, int amount)>
        {
            (input.ItemKey, requiredAmount * amount)
        };
        if (!gameManager.Inventory.UseCraftingMaterials(reqList)) return;

        var outData = dataManager.ItemLoader.GetItemByKey(output.ItemKey);
        if (outData != null)
            gameManager.Inventory.AddItem(outData, resultAmount * amount);

        gameManager.Forge.AddGold(-cost);

        selectedMaterials[index] = null;
        UpdateSlot(index);
    }

    private void ResetUI()
    {
        for (int i = 0; i < refineSlots.Count; i++)
        {
            refineSlots[i].SetAmount(1);
            selectedMaterials[i] = null;
            UpdateSlot(i);
        }
    }

    public override void Open()
    {
        base.Open();
        ResetUI();
    }

    public override void Close()
    {
        base.Close();
    }
}
