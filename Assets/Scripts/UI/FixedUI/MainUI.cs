using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Vector3 selectedScale;
    [SerializeField] private Vector3 defaultScale = Vector3.one;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("Top Info")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image levelFill;
    [SerializeField] private TextMeshProUGUI fameText;
    [SerializeField] private TextMeshProUGUI forgeNameText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI diaText;

    public override UIType UIType => UIType.Fixed;

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        SwitchTab(2);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;

            tabPanels[i].SetActive(isSelected);

            tabButtons[i].transform.localScale = isSelected ? selectedScale : defaultScale;
            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }

        uIManager.CloseAllWindowUI();
    }
}
