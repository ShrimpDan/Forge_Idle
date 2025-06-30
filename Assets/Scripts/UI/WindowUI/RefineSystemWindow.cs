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
    private int refineCost = 1000;                        // 임시 비용

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.gameManager = gameManager;
        this.uIManager = uIManager;

        // 버튼 리스너
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
        // 인벤토리에서 "리소스" 탭만 노출
        var popup = uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
        popup.SetResourceSelectCallback(OnMaterialSelected);
    }

    private void OnMaterialSelected(ItemInstance item)
    {
        selectedMaterial = item;

        // 아이콘 세팅
        if (inputSlotIcon != null && item?.Data != null)
        {
            inputSlotIcon.sprite = IconLoader.GetIcon(item.Data.IconPath);
            inputSlotIcon.enabled = true;
        }
        else if (inputSlotIcon != null)
        {
            inputSlotIcon.sprite = null;
            inputSlotIcon.enabled = false;
        }

        // 결과 (실제 변환은 나중 구현)
        resultItem = null; // 실제 정련 결과 아이템 예측용 더미
        UpdateOutputSlot();
        UpdateCost();
    }

    private void UpdateOutputSlot()
    {
        // 결과 아이템 프리뷰
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
        // 실제 비용 차감/결과 처리 구현 예정

        // 정련 로직 자리 (추후 구현)
        Debug.Log("[RefineSystem] 정련 완료! (실제 변환 로직은 추후 구현)");

        // UI 초기화
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
