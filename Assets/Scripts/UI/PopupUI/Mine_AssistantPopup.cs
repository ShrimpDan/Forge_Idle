using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Mine_AssistantPopup : BaseUI
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
    [SerializeField] private Button assignButton;
    [SerializeField] private Button unassignButton;
    [SerializeField] private Button exitButton;

    private Action<AssistantInstance, bool> onAssignToggleCallback;
    private AssistantInstance assiData;
    private bool isAssigned;
    private UIManager uiManager;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.Mine_AssistantPopup));
        assignButton.onClick.RemoveAllListeners();
        assignButton.onClick.AddListener(OnAssign);
        unassignButton.onClick.RemoveAllListeners();
        unassignButton.onClick.AddListener(OnUnassign);
    }

    public void SetAssistant(AssistantInstance data, bool isAlreadyAssigned, Action<AssistantInstance, bool> onAssignToggle)
    {
        assiData = data;
        isAssigned = isAlreadyAssigned;
        onAssignToggleCallback = onAssignToggle;

        assiName.text = data.Name;
        assiType.text = data.Specialization.ToString();

        foreach (Transform child in optionRoot)
            Destroy(child.gameObject);

        foreach (var option in data.Multipliers)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);
            TextMeshProUGUI optionText = obj.GetComponent<TextMeshProUGUI>();
            optionText.text = $"{option.AbilityName}\nx{option.Multiplier}";
        }

        assignButton.gameObject.SetActive(!isAssigned);
        unassignButton.gameObject.SetActive(isAssigned);
    }

    private void OnAssign()
    {
        onAssignToggleCallback?.Invoke(assiData, true);
        // �˾� ��� ����
        if (uiManager != null)
            uiManager.CloseUI(UIName.Mine_AssistantPopup);
    }

    private void OnUnassign()
    {
        onAssignToggleCallback?.Invoke(assiData, false);
        if (uiManager != null)
            uiManager.CloseUI(UIName.Mine_AssistantPopup);
    }
}
