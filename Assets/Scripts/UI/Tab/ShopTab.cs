using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopTab : BaseTab
{
    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor = Color.grey;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        foreach (var tab in tabPanels)
        {
            if (tab.TryGetComponent(out BaseTab baseTab))
            {
                baseTab.Init(gameManager, uIManager);
            }
        }
        
        SwitchTab(0);
    }

    public override void OpenTab()
    {
        base.OpenTab();
    }

    public override void CloseTab()
    {
        base.CloseTab();
    }
    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;

            tabPanels[i].SetActive(isSelected);

            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }

        SoundManager.Instance.Play("SFX_SystemClick");

    }
}


