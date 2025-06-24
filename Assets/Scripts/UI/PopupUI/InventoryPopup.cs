using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContext
{
    public Sprite Icon;
    public string Name;
    public int Count;
    public string Description;

    public List<(string label, UnityEngine.Events.UnityAction action)> Actions = new();
}

public class InventoryPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI objectName;
    [SerializeField] private TextMeshProUGUI valueType;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button exitButton;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.InventoryPopup));
    }

    public void SetContext(InventoryContext context)
    {
        icon.sprite = context.Icon;
        objectName.text = context.Name;
        value.text = context.Count.ToString();
        description.text = context.Description;

        foreach (var btnAction in context.Actions)
        {
            CreateButton(btnAction.label, btnAction.action);
        }
    }

    private void CreateButton(string label, UnityEngine.Events.UnityAction action)
    {
        GameObject go = Instantiate(buttonPrefab, buttonContainer);
        var button = go.GetComponent<Button>();
        var btnName = go.GetComponentInChildren<TextMeshProUGUI>();

        btnName.text = label;
        button.onClick.AddListener(action);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }
}
