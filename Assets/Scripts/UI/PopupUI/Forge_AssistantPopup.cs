using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Forge_AssistantPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [Header("Assistant Info UI")]
    [SerializeField] private Image icon;
    [SerializeField] private Image typeIcon;
    [SerializeField] private TextMeshProUGUI assiName;
    [SerializeField] private TextMeshProUGUI assiType;

    [Header("Assistant Option Info")]
    [SerializeField] private GameObject optionTextPrefab;
    [SerializeField] private Transform optionRoot;

    [Header("Button UI")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button deApplyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button iconSlotBtn;

    private TraineeData assiData;

    public override void Init(GameManager gameManager, UIManager uIManager) // ★ 수정 : override
    {
        base.Init(gameManager, uIManager);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.AssistantPopup));

        applyButton.onClick.RemoveAllListeners();
        applyButton.onClick.AddListener(ApplyAssistant);

        deApplyButton.onClick.RemoveAllListeners();
        deApplyButton.onClick.AddListener(DeApplyAssistant);

        if (iconSlotBtn != null)
        {
            iconSlotBtn.onClick.RemoveAllListeners();
            iconSlotBtn.onClick.AddListener(OpenInventoryPopup);
        }
    }

    private void OpenInventoryPopup()
    {
        // 싱글턴 Instance로 하지 말고, BaseUI의 uIManager 멤버를 씁니다!
        uIManager.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
    }

    public override void Open()
    {
        base.Open();
    }

    public void SetAssistant(TraineeData data)
    {
        assiData = data;

        assiName.text = data.Name;
        assiType.text = data.Specialization.ToString();

        // 옵션 텍스트 초기화
        foreach (Transform child in optionRoot)
            Destroy(child.gameObject);

        foreach (var option in data.Multipliers)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);
            TextMeshProUGUI optionText = obj.GetComponent<TextMeshProUGUI>();
            optionText.text = option.AbilityName;
            optionText.text += $"\nx{option.Multiplier}";
        }

        SetApplyButton();
    }

    public override void Close()
    {
        base.Close();
        exitButton.onClick.RemoveAllListeners();
        applyButton.onClick.RemoveAllListeners();
        deApplyButton.onClick.RemoveAllListeners();
        if (iconSlotBtn != null) iconSlotBtn.onClick.RemoveAllListeners();
    }

    private void ApplyAssistant()
    {
        assiData.IsEquipped = true;
        SetApplyButton();
    }

    private void DeApplyAssistant()
    {
        assiData.IsEquipped = false;
        SetApplyButton();
    }

    private void SetApplyButton()
    {
        if (assiData != null && assiData.IsEquipped)
        {
            applyButton.gameObject.SetActive(false);
            deApplyButton.gameObject.SetActive(true);
        }
        else
        {
            applyButton.gameObject.SetActive(true);
            deApplyButton.gameObject.SetActive(false);
        }
    }
}
