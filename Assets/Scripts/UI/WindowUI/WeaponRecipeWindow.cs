using UnityEngine;
using UnityEngine.UI;

public class WeaponRecipeWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("UI Elements")]
    [SerializeField] Button exitBtn;

    private Forge forge;
    
    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forge = gameManager.Forge;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("SFX_SystemClick");
                SwitchTab(index);
            });
        }

        SwitchTab(0);
        
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.GetRecipeWindowByType(forge.ForgeType)));
    }

    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;

            tabPanels[i].SetActive(isSelected);

            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }
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
