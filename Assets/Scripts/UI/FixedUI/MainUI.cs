using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    private ForgeManager forgeManager;

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
    [SerializeField] private Image fameFill;
    [SerializeField] private TextMeshProUGUI fameText;
    [SerializeField] private TextMeshProUGUI forgeNameText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI diaText;

    public override UIType UIType => UIType.Fixed;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forgeManager = gameManager.ForgeManager;

        foreach (var tab in tabPanels)
        {
            if (tab.TryGetComponent(out BaseTab tabUI))
            {
                tabUI.Init(gameManager, uIManager);
            }
        }

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("SFX_SystemClick");
                SwitchTab(index);
            });
        }

        OnEnable();
    }

    public override void Open()
    {
        base.Open();
        SwitchTab(2);
    }
    
    void OnEnable()
    {
        if (forgeManager == null) return;

        forgeManager.Events.OnGoldChanged += SetGoldUI;
        forgeManager.Events.OnDiaChanged += SetDiaUI;
        forgeManager.Events.OnLevelChanged += SetLevelUI;
        forgeManager.Events.OnFameChanged += SetFameBarUI;
        forgeManager.Events.OnTotalFameChanged += SetTotalFameUI;
    }

    void OnDisable()
    {
        forgeManager.Events.OnGoldChanged -= SetGoldUI;
        forgeManager.Events.OnDiaChanged -= SetDiaUI;
        forgeManager.Events.OnLevelChanged -= SetLevelUI;
        forgeManager.Events.OnFameChanged -= SetFameBarUI;
        forgeManager.Events.OnTotalFameChanged -= SetTotalFameUI;
    }

    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;

            if (tabPanels[i].TryGetComponent(out BaseTab tabUI))
            {
                if (isSelected)
                    tabUI.OpenTab();
                else
                    tabUI.CloseTab();
            }
            else
            {
                tabPanels[i].SetActive(isSelected);
            }

            tabButtons[i].transform.localScale = isSelected ? selectedScale : defaultScale;
            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }

        uIManager.CloseAllWindowUI();
    }

    private void SetGoldUI(int gold)
    {
        goldText.text = UIManager.FormatNumber(gold);
    }

    private void SetDiaUI(int dia)
    {
        diaText.text = UIManager.FormatNumber(dia);
    }

    private void SetLevelUI(int level)
    {
        levelText.text = level.ToString();
    }

    private void SetFameBarUI(int curFame, int maxFame)
    {
        fameFill.fillAmount = (float)curFame / maxFame;
    }

    private void SetTotalFameUI(int totalFame)
    {
        fameText.text = $"Fame: {UIManager.FormatNumber(totalFame)}";
    }
}
