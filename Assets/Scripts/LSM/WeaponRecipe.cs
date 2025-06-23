using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponRecipe", menuName = "Game/WeaponRecipe")]
public class WeaponRecipe : ScriptableObject
{
    [System.Serializable]
    public class MaterialRequirement
    {
        public MaterialItem material;
        public int quantity;
    }

    public List<MaterialRequirement> requirements;
    public Weapon resultWeapon;
    public int goldRequired;
}
