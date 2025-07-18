using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour
{
    private WeaponRecipeSystem recipeSystem;

    private Button slotBtn;

    public void Init(WeaponRecipeSystem recipeSystem)
    {
        this.recipeSystem = recipeSystem;
    }
}
