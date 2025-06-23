using UnityEngine;

public class WeaponCraftingSystem : MonoBehaviour
{
    public string CraftWeapon(WeaponRecipe recipe)
    {
        foreach (var req in recipe.requirements)
        {
            if (!InventoryManager.Instance.HasEnoughItems(req.material, req.quantity))
            {
                return $"재료 부족: {req.material.itemName}";
            }
        }

        if (!InventoryManager.Instance.SpendGold(recipe.goldRequired))
        {
            return $"골드 부족! (필요 골드: {recipe.goldRequired})";
        }

        foreach (var req in recipe.requirements)
        {
            for (int i = 0; i < req.quantity; i++)
            {
                InventoryManager.Instance.RemoveItem(req.material);
            }
        }

        Weapon weaponInstance = Instantiate(recipe.resultWeapon);
        InventoryManager.Instance.AddItem(weaponInstance);

        return $"무기 제작 성공! {weaponInstance.itemName} 제작 완료";
    }
}
