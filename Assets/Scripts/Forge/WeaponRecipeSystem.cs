using System.Collections.Generic;

public class WeaponRecipeSystem
{
    private Forge forge;
    private ForgeManager forgeManager;
    private CraftingDataLoader craftingLoader;

    public Dictionary<WeaponType, List<string>> UnlockedRecipeDict { get; private set; }
    public int CurRecipePoint  { get; private set; }
    public int TotalRecipePoint { get; private set; }
    private int UsedPoint => TotalRecipePoint - CurRecipePoint;
    private int resetGold; 

    public WeaponRecipeSystem(Forge forge, CraftingDataLoader dataLoader)
    {
        this.forge = forge;
        forgeManager = forge.ForgeManager;
        craftingLoader = dataLoader;

        UnlockedRecipeDict = new Dictionary<WeaponType, List<string>>();
        WeaponType[] weaponTypes = ForgeWeaponTypeMapping.ForgeWeaponTypeDict[forge.ForgeType];

        foreach (var type in weaponTypes)
        {
            UnlockedRecipeDict[type] = new List<string>();
        }
    }


    public void AddPoint(int amount)
    {
        CurRecipePoint += amount;
        TotalRecipePoint += amount;
    }

    public bool UsePoint(int amount)
    {
        if (CurRecipePoint - amount < 0)
        {
            return false;
        }

        CurRecipePoint -= amount;
        return true;
    }

    public void ResetPoint()
    {
        if (forgeManager.UseGold(resetGold * UsedPoint))
        {
            CurRecipePoint = TotalRecipePoint;

            foreach (var key in UnlockedRecipeDict.Keys)
            {
                UnlockedRecipeDict[key].Clear();
            }
        }
    }

    public void UnlockRecipe(WeaponType type, string recipeKey)
    {
        UnlockedRecipeDict[type].Add(recipeKey);
    }

    public List<string> GetKeysByType(WeaponType type)
    {
        return UnlockedRecipeDict[type];
    }

    public List<CraftingData> GetDatasByType(WeaponType type)
    {
        if (UnlockedRecipeDict[type].Count > 0)
        {
            List<CraftingData> dataLists = new List<CraftingData>();

            foreach (var key in UnlockedRecipeDict[type])
            {
                dataLists.Add(craftingLoader.GetDataByKey(key));
            }

            return dataLists;
        }
        
        return null;
    }
}
