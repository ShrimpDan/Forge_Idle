using UnityEngine;
using UnityEngine.UI;

public class WeaponRecipeSlot : MonoBehaviour
{
    private WeaponRecipeSystem recipeSystem;
    private WeaponRecipeWindow recipeWindow;

    private Button slotBtn;
    private string recipeKey;

    public void Init(WeaponRecipeSystem recipeSystem, WeaponRecipeWindow recipeWindow)
    {
        this.recipeSystem = recipeSystem;
        this.recipeWindow = recipeWindow;
        recipeKey = name;

        if (slotBtn == null)
        {
            slotBtn = GetComponent<Button>();
            slotBtn.onClick.AddListener(ClickRecipe);
        }

        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        bool isUnlock = recipeSystem.CheckUnlock(recipeKey);
        slotBtn.image.color = isUnlock ? Color.white : Color.grey;
    }

    private void ClickRecipe()
    {
        recipeWindow.SetInfoUI(this, recipeKey);
    }
}
