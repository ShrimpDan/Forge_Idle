using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssistantPopup : BaseUI
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

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        exitButton.onClick.AddListener(() => uIManager.CloseUI(UIName.AssistantPopup));
    }

    public override void Open()
    {
        base.Open();


    }

    public void SetUI(TestAssistantData data)
    {
        // 아이콘 설정

        assiName.text = data.Name;
        assiType.text = data.Type;

        foreach (var option in data.OptionList)
        {
            GameObject obj = Instantiate(optionTextPrefab, optionRoot);

            if (obj.TryGetComponent(out AssistantSlot assiSlot))
            {
                
            }
        }
    }

    public override void Close()
    {
        base.Close();
        exitButton.onClick.RemoveAllListeners();
    }
}
