using System.Collections.Generic;

public class WeaponRecipeSystem
{
    private Forge forge;
    private ForgeManager forgeManager;
    private CraftingDataLoader craftingLoader;

    public Dictionary<WeaponType, List<string>> UnlockedRecipeDict { get; private set; } // 이거 무기타입별 해금된 제작법
    public int CurRecipePoint { get; private set; }
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

    public void UnlockRecipe(CraftingRecipeData recipeData)
    {
        if (UsePoint(recipeData.NeedPoint))
        {
            UnlockedRecipeDict[recipeData.Type].Add(recipeData.Key);
            return;
        }

        // 포인트 부족 알림
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

    public bool CheckUnlock(CraftingRecipeData data)
    {
        if (UnlockedRecipeDict[data.Type].Contains(data.Key))
            return true;

        return false;
    }
}
