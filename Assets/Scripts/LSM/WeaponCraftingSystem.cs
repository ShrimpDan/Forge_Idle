using UnityEngine;

public class WeaponCraftingSystem : MonoBehaviour
{
    public string CraftWeapon(WeaponRecipe recipe)
    {
        foreach (var req in recipe.requirements)
        {
            if (!InventoryManager.Instance.HasEnoughItems(req.material, req.quantity))
            {
                return $"��� ����: {req.material.itemName}";
            }
        }

        if (!InventoryManager.Instance.SpendGold(recipe.goldRequired))
        {
            return $"��� ����! (�ʿ� ���: {recipe.goldRequired})";
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

        return $"���� ���� ����! {weaponInstance.itemName} ���� �Ϸ�";
    }
}
