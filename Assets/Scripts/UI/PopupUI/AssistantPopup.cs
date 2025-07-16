using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssistantPopup : BaseUI
{
    private Forge forge;
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

    AssistantInstance assiData;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        forge = gameManager.Forge;

        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.AssistantPopup));
        applyButton.onClick.AddListener(ApplyAssistant);
        deApplyButton.onClick.AddListener(DeApplyAssistant);
    }

    public override void Open()
    {
        base.Open();
    }

    public void SetAssistant(AssistantInstance data)
    {
        assiData = data;
        // 캐릭터 아이콘 & 타입별 아이콘 설정

        icon.sprite = IconLoader.GetIcon(data.IconPath);
        assiName.text = data.Name;
        assiType.text = data.Specialization.ToString();

        foreach (var option in data.Multipliers)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);

            TextMeshProUGUI optionText = obj.GetComponent<TextMeshProUGUI>();
            optionText.text = option.AbilityName;
            optionText.text += $"\nx{option.Multiplier}";
        }

        SetApplyButton(data.IsEquipped);
    }

    public override void Close()
    {
        base.Close();
        exitButton.onClick.RemoveAllListeners();
    }

    private void ApplyAssistant()
    {
        forge.AssistantHandler.ActiveAssistant(assiData);
        SetApplyButton(true);
    }

    private void DeApplyAssistant()
    {
        forge.AssistantHandler.DeActiveAssistant(assiData);
        SetApplyButton(false);
    }

    private void SetApplyButton(bool isEquip)
    {
        if (isEquip)
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
