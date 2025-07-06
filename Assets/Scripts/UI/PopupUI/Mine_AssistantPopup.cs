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

    private UIManager uiManager;
    private TraineeData assiData;
    private Action<TraineeData, bool> onAssignToggleCallback; 
    private bool isAssigned;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        this.uiManager = uIManager;

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uiManager.CloseUI(UIName.Mine_AssistantPopup));

        assignButton.onClick.RemoveAllListeners();
        assignButton.onClick.AddListener(OnAssign);

        unassignButton.onClick.RemoveAllListeners();
        unassignButton.onClick.AddListener(OnUnassign);
    }


    public void SetAssistant(TraineeData data, bool isAlreadyAssigned, Action<TraineeData, bool> onAssignToggle)
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

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        assignButton.gameObject.SetActive(!isAssigned);
        unassignButton.gameObject.SetActive(isAssigned);
    }

    private void OnAssign()
    {
        onAssignToggleCallback?.Invoke(assiData, true); 
        isAssigned = true;
        UpdateButtonState();
        uiManager.CloseUI(UIName.Mine_AssistantPopup);
    }

    private void OnUnassign()
    {
        onAssignToggleCallback?.Invoke(assiData, false); 
        isAssigned = false;
        UpdateButtonState();
        uiManager.CloseUI(UIName.Mine_AssistantPopup);
    }
}
