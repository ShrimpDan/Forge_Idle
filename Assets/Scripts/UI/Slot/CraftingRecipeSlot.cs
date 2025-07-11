using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour
{
    private WeaponRecipeSystem recipeSystem;

    [SerializeField] private CraftingRecipeSO recipeSO;
    private Button slotBtn;

    public void Init(WeaponRecipeSystem recipeSystem)
    {
        this.recipeSystem = recipeSystem;
    }
}
