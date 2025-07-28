using System.Collections.Generic;

public enum ItemType
{
    Resource = 0,
    Weapon = 1,
    Gem = 2,
    Ingot = 3
}

public enum WeaponType
{
    OneHanded_Sword = 0,
    TwoHanded_Sword,
    Hammer,
    Mace,
    Axe,
    Long_Bow,
    Short_Bow,
    Gun,
    Light_Plate,
    Heavy_Plate,
    Chain_Plate,
    Light_Helmet,
    Heavy_Helmet,
    Small_Shield,
    Big_Shield,
    Wand,
    Staff,
    Ring,
    Necklace
}

public enum ForgeType
{
    Weapon = 0,
    Armor = 1,
    Magic = 2,
    None = 3
}

public enum ForgeUpgradeType
{
    IncreaseSellPrice = 0,
    IncreaseExpensiveRecipeChance = 1,
    ReduceCustomerSpawnDelay = 2,
    ReduceAutoCraftingTime = 3,
    IncreasePerfectCraftChance = 4,
    UpgradeInterior = 5
}

public static class ForgeWeaponTypeMapping
{
    public static readonly Dictionary<ForgeType, WeaponType[]> ForgeWeaponTypeDict = new Dictionary<ForgeType, WeaponType[]>
    {
        {
            ForgeType.Weapon, new WeaponType[]
            {
                WeaponType.OneHanded_Sword,
                WeaponType.TwoHanded_Sword,
                WeaponType.Hammer,
                WeaponType.Mace,
                WeaponType.Axe,
                WeaponType.Long_Bow,
                WeaponType.Short_Bow,
                WeaponType.Gun
            }
        },
        {
            ForgeType.Armor, new WeaponType[]
            {
                WeaponType.Light_Plate,
                WeaponType.Heavy_Plate,
                WeaponType.Chain_Plate,
                WeaponType.Light_Helmet,
                WeaponType.Heavy_Helmet,
                WeaponType.Small_Shield,
                WeaponType.Big_Shield
            }
        },
        {
            ForgeType.Magic, new WeaponType[]
            {
                WeaponType.Wand,
                WeaponType.Staff,
                WeaponType.Ring,
                WeaponType.Necklace
            }
        }
    };
}