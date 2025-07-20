using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRecipeWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    private Forge forge;
    private WeaponRecipeSystem recipeSystem;
    private DataManager dataManager;

    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Button exitBtn;

    [Header("RecipeSlots Root")]
    [SerializeField] private WeaponRecipeSlot[] recipeSlots;

    private CraftingRecipeData selectedRecipe;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forge = gameManager.Forge;
        recipeSystem = forge.RecipeSystem;
        dataManager = gameManager.DataManager;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("SFX_SystemClick");
                SwitchTab(index);
            });
        }

        foreach (var slot in recipeSlots)
        {
            slot.Init(recipeSystem, this);
        }
        SwitchTab(0);

        unlockBtn.onClick.RemoveAllListeners();
        unlockBtn.onClick.AddListener(ClickUnlockBtn);

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

    public void SetInfoUI(string key)
    {
        selectedRecipe = dataManager.RecipeLoader.GetDataByKey(key);
        ItemData itemData = dataManager.ItemLoader.GetItemByKey(key);

        icon.sprite = IconLoader.GetIconByKey(key);
        nameText.text = itemData.Name;
        pointText.text = selectedRecipe.NeedPoint.ToString();

        if (recipeSystem.CheckUnlock(selectedRecipe))
            unlockBtn.interactable = false;
        else
            unlockBtn.interactable = true;
    }

    private void ClickUnlockBtn()
    {
        recipeSystem.UnlockRecipe(selectedRecipe);
        SetInfoUI(selectedRecipe.Key);
    }
}
