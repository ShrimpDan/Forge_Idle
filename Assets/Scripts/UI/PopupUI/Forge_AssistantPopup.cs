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

    TraineeData assiData;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.AssistantPopup));
        applyButton.onClick.AddListener(ApplyAssistant);
        deApplyButton.onClick.AddListener(DeApplyAssistant);

        if (iconSlotBtn != null)
            iconSlotBtn.onClick.AddListener(OpenInventoryPopup);
    }

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
    }

    private void OpenInventoryPopup()
    {
        UIManager.Instance.OpenUI<Forge_Inventory_Popup>(UIName.Forge_Inventory_Popup);
    }

    public override void Open()
    {
        base.Open();
    }

    public void SetAssistant(TraineeData data)
    {
        assiData = data;
        // 캐릭터 아이콘 & 타입별 아이콘 설정

        assiName.text = data.Name;
        assiType.text = data.Specialization.ToString();

        foreach (var option in data.Multipliers)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);

            TextMeshProUGUI optionText = obj.GetComponent<TextMeshProUGUI>();
            optionText.text = option.AbilityName;
            optionText.text += $"\nx{option.Multiplier}";
        }
    }

    public override void Close()
    {
        base.Close();
        exitButton.onClick.RemoveAllListeners();
    }

    private void ApplyAssistant()
    {
        assiData.IsEquipped = true;
    }

    private void DeApplyAssistant()
    {
        assiData.IsEquipped = false;
    }

    private void SetApplyButton()
    {
        if (assiData.IsEquipped)
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
