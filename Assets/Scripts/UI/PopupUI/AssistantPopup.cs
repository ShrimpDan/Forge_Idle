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
    [SerializeField] private TextMeshProUGUI personalityText;
    [SerializeField] private GameObject equippedIndicator;
    [SerializeField] private GameObject firedIndicator;

    [Header("Assistant Option Info")]
    [SerializeField] private GameObject optionTextPrefab;
    [SerializeField] private Transform optionRoot;

    [Header("Button UI")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button deApplyButton;
    [SerializeField] private Button rehireButton;
    [SerializeField] private Button exitButton;

    private AssistantTab assistantTab;
    private AssistantInstance assiData;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        forge = gameManager.Forge;

        if (assistantTab == null)
            assistantTab = uIManager.GetComponentInChildren<AssistantTab>(true);

        applyButton.onClick.AddListener(ApplyAssistant);
        deApplyButton.onClick.AddListener(DeApplyAssistant);
        rehireButton.onClick.AddListener(RehireAssistant);
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.AssistantPopup));
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        exitButton.onClick.RemoveAllListeners();
    }

    public void SetAssistant(AssistantInstance data)
    {
        assiData = data;

        // 아이콘, 텍스트
        icon.sprite = IconLoader.GetIconByPath(data.IconPath);
        assiName.text = data.Name;
        typeIcon.sprite = IconLoader.GetIconByPath($"Icons/Specializations/{data.Specialization}");
        assiType.text = FusionSlotView.GetKoreanSpecialization(data.Specialization);

        if (personalityText != null)
            personalityText.text = data.Personality?.personalityName ?? "알 수 없음";

        // 옵션 초기화
        foreach (Transform child in optionRoot)
            Destroy(child.gameObject);

        if (data.IsFired)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);
            var optionText = obj.GetComponent<TextMeshProUGUI>();
            optionText.text = $"재고용 비용 : {data.RehireCost} G";

            if (obj.GetComponent<LayoutElement>() == null)
            {
                var layout = obj.AddComponent<LayoutElement>();
                layout.preferredHeight = 40f;
            }
        }
        else
        {
            foreach (var option in data.Multipliers)
            {
                if (option.Multiplier == 0) continue;

                GameObject obj = Instantiate(optionTextPrefab, optionRoot);
                var optionText = obj.GetComponent<TextMeshProUGUI>();

                float value = option.Multiplier * 100f;
                optionText.text = $"{option.AbilityName}\n+{value:F0}%";

                if (obj.GetComponent<LayoutElement>() == null)
                {
                    var layout = obj.AddComponent<LayoutElement>();
                    layout.preferredHeight = 40f;
                }
            }
        }

        RefreshEquippedState();
        SetApplyButton(data);
    }

    private void SetApplyButton(AssistantInstance data)
    {
        bool isEquipped = data.IsEquipped;
        bool isFired = data.IsFired;
        SpecializationType type = data.Specialization;

        applyButton.gameObject.SetActive(false);
        deApplyButton.gameObject.SetActive(false);
        rehireButton.gameObject.SetActive(false);

        if (type == SpecializationType.Mining)
        {
            if (isFired)
            {
                rehireButton.gameObject.SetActive(true);
            }
            return;
        }

        if (isFired)
        {
            rehireButton.gameObject.SetActive(true);
        }
        else if (isEquipped)
        {
            deApplyButton.gameObject.SetActive(true);
        }
        else
        {
            applyButton.gameObject.SetActive(true);
        }
    }

    private void ApplyAssistant()
    {
        if (assiData == null) return;

        if (!assiData.IsActive)
        {
            Debug.LogWarning("탈주한 제자는 착용할 수 없습니다.");
            return;
        }

        forge.AssistantHandler.ActiveAssistant(assiData);

        RefreshEquippedState();
        SetApplyButton(assiData);
        assistantTab?.RefreshSlots();
        SoundManager.Instance.Play("SFX_UIEquip");
    }

    private void DeApplyAssistant()
    {
        if (assiData == null) return;

        forge.AssistantHandler.DeActiveAssistant(assiData);
        RefreshEquippedState();
        SetApplyButton(assiData);

        assistantTab?.RefreshSlots();
        SoundManager.Instance.Play("SFX_UIUnequip");
    }

    private void RehireAssistant()
    {
        if (assiData == null || !assiData.IsFired)
            return;

        int cost = assiData.RehireCost;
        if (GameManager.Instance.ForgeManager.UseGold(cost))
        {
            assiData.IsFired = false;
            GameManager.Instance.SaveManager.SaveAll();
            Debug.Log($"{assiData.Name} 재고용 완료!");

            SetAssistant(assiData);

            assistantTab?.RefreshSlots();
        }
        else
        {
            Debug.LogWarning("골드가 부족합니다.");
        }
    }

    private void RefreshEquippedState()
    {
        if (assiData == null) return;

        bool isEquipped = assiData.IsEquipped;
        bool isFired = assiData.IsFired;

        if (equippedIndicator != null)
            equippedIndicator.SetActive(isEquipped && !isFired);

        if (firedIndicator != null)
            firedIndicator.SetActive(isFired);
    }
}
