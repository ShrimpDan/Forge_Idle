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

    public override void Init(GameManager gameManager, UIManager uIManager)
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
            iconSlotBtn.onClick.AddListener(OpenRecipePopup);
        }
    }

    // ** 수정: 레시피 팝업을 띄울 때 반드시 Init을 호출하고, Init에서 Open 실행하도록 강제 **
    private void OpenRecipePopup()
    {
        var popup = uIManager.OpenUI<Forge_Recipe_Popup>(UIName.Forge_Recipe_Popup);
        popup.Init(gameManager.TestDataManager, uIManager); // Init 내부에서 Open() 실행
    }

    public override void Open() => base.Open();

    public void SetAssistant(TraineeData data)
    {
        assiData = data;
        assiName.text = data.Name;
        assiType.text = data.Specialization.ToString();

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
