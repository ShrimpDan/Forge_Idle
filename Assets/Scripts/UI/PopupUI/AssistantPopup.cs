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

    private AssistantInstance assiData;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        forge = gameManager.Forge;

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
        assiType.text = data.Specialization.ToString();

        // 옵션 초기화
        foreach (Transform child in optionRoot)
            Destroy(child.gameObject);

        if (data.IsFired)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);
            var optionText = obj.GetComponent<TextMeshProUGUI>();
            optionText.text = $"재고용 비용 : {data.RehireCost} G";
        }
        else
        {
            foreach (var option in data.Multipliers)
            {
                GameObject obj = Instantiate(optionTextPrefab, optionRoot);
                var optionText = obj.GetComponent<TextMeshProUGUI>();
                optionText.text = $"{option.AbilityName}\nx{option.Multiplier}";
            }
        }

        RefreshEquippedState(); // ← 장착 상태 UI 반영
        SetApplyButton(data);   // ← 버튼 상태 반영
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
        SetApplyButton(assiData);
    }

    private void DeApplyAssistant()
    {
        if (assiData == null) return;

        forge.AssistantHandler.DeActiveAssistant(assiData);
        RefreshEquippedState();
        SetApplyButton(assiData);
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
            RefreshEquippedState();
            SetApplyButton(assiData);
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
