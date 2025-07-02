using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RefineSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button inputSlotButton;
    [SerializeField] private Image inputSlotIcon;
    [SerializeField] private Image outputSlotIcon;
    [SerializeField] private TMP_Text refineCostText;
    [SerializeField] private Button executeButton;

    private ItemInstance selectedMaterial;
    private ItemInstance resultItem;
    private GameManager gameManager;
    private UIManager uIManager;
    private DataManager dataManager;

    private int refineCost = 1000;
    private const int requiredAmount = 5;
    private const int resultAmount = 1;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;
        this.dataManager = gameManager?.DataManager;

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.RefineSystemWindow));

        inputSlotButton.onClick.RemoveAllListeners();
        inputSlotButton.onClick.AddListener(OnClickInputSlot);

        executeButton.onClick.RemoveAllListeners();
        executeButton.onClick.AddListener(OnClickExecuteRefine);

        ResetUI();
    }

    private void OnClickInputSlot()
    {
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetResourceSelectCallback(OnMaterialSelected);
    }

    private void OnMaterialSelected(ItemInstance item)
    {
        selectedMaterial = item;
        if (selectedMaterial != null && selectedMaterial.Data == null && dataManager != null)
            selectedMaterial.Data = dataManager.ItemLoader.GetItemByKey(selectedMaterial.ItemKey);

        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (inputSlotIcon != null)
        {
            inputSlotIcon.sprite = selectedMaterial?.Data != null ? IconLoader.GetIcon(selectedMaterial.Data.IconPath) : null;
            inputSlotIcon.enabled = selectedMaterial?.Data != null;
        }

        resultItem = GetRefineResult(selectedMaterial);

        if (outputSlotIcon != null)
        {
            outputSlotIcon.sprite = resultItem?.Data != null ? IconLoader.GetIcon(resultItem.Data.IconPath) : null;
            outputSlotIcon.enabled = resultItem?.Data != null;
        }

        UpdateCost();
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

    private void UpdateCost()
    {
        if (refineCostText != null)
            refineCostText.text = $"정련 비용: {refineCost:N0} 골드";
    }

    private void OnClickExecuteRefine()
    {
        if (selectedMaterial == null || resultItem == null) return;
        if (gameManager.Forge.Gold < refineCost) return;

        int owned = gameManager.Inventory.ResourceList
            .Where(x => x.ItemKey == selectedMaterial.ItemKey)
            .Sum(x => x.Quantity);
        if (owned < requiredAmount) return;

        // 골드 차감
        gameManager.Forge.AddGold(-refineCost);

        // 재료 차감
        var reqList = new System.Collections.Generic.List<(string resourceKey, int amount)>()
        {
            (selectedMaterial.ItemKey, requiredAmount)
        };
        gameManager.Inventory.UseCraftingMaterials(reqList);

        // 결과 아이템 생성 및 추가 (핵심)
        var outData = dataManager.ItemLoader.GetItemByKey(resultItem.ItemKey);
        if (outData == null) return;
        gameManager.Inventory.AddItem(outData, resultAmount);

        ResetUI();
    }

    private void ResetUI()
    {
        selectedMaterial = null;
        resultItem = null;
        if (inputSlotIcon != null)
        {
            inputSlotIcon.sprite = null;
            inputSlotIcon.enabled = false;
        }
        if (outputSlotIcon != null)
        {
            outputSlotIcon.sprite = null;
            outputSlotIcon.enabled = false;
        }
        UpdateCost();
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
