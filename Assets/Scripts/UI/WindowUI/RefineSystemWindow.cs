using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private DataManger dataManager;

    private int refineCost = 1000; 

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

        if (executeButton != null)
        {
            executeButton.onClick.RemoveAllListeners();
            executeButton.onClick.AddListener(OnClickExecuteRefine);
        }

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
        {
            // 혹시라도 Data가 비어있으면 DataLoader에서 보충
            selectedMaterial.Data = dataManager.ItemLoader.GetItemByKey(selectedMaterial.ItemKey);
        }

        // 아이콘 세팅
        if (inputSlotIcon != null && selectedMaterial?.Data != null)
        {
            inputSlotIcon.sprite = IconLoader.GetIcon(selectedMaterial.Data.IconPath);
            inputSlotIcon.enabled = true;
        }
        else if (inputSlotIcon != null)
        {
            inputSlotIcon.sprite = null;
            inputSlotIcon.enabled = false;
        }

        // --- 정련 결과 아이템 미리보기 ---
        resultItem = null;

        UpdateOutputSlot();
        UpdateCost();
    }

    private void UpdateOutputSlot()
    {
        if (outputSlotIcon != null)
        {
            if (resultItem != null && resultItem.Data != null)
            {
                outputSlotIcon.sprite = IconLoader.GetIcon(resultItem.Data.IconPath);
                outputSlotIcon.enabled = true;
            }
            else
            {
                outputSlotIcon.sprite = null;
                outputSlotIcon.enabled = false;
            }
        }
    }

    private void UpdateCost()
    {
        if (refineCostText)
            refineCostText.text = $"정련 비용: {refineCost} 골드";
    }

    private void OnClickExecuteRefine()
    {
        if (selectedMaterial == null)
        {
            Debug.LogWarning("[RefineSystem] 재료를 먼저 선택하세요!");
            return;
        }

        // --- 실제 비용 차감/정련 처리 자리 ---
        // 예시: gameManager.Forge.AddGold(-refineCost);
        // 실제로는 selectedMaterial을 소모하고 resultItem을 인벤토리에 추가

        Debug.Log("[RefineSystem] 정련 완료! (정련 결과 지급 로직 자리)");

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
