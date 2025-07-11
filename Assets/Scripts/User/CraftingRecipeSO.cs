using UnityEngine;

[CreateAssetMenu(fileName = "recipe", menuName = "WeaponRecipe/New Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
    public WeaponType type;
    public string recipeKey;
    public int needPoint;
}
