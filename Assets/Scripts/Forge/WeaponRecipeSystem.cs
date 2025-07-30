using System.Collections.Generic;

public class WeaponRecipeSystem
{
    private Forge forge;
    private ForgeManager forgeManager;
    private CraftingDataLoader craftingLoader;
    private CraftingRecipeLoader recipeLoader;

    public Dictionary<WeaponType, List<string>> UnlockedRecipeDict { get; private set; } // 이거 무기타입별 해금된 제작법

    public WeaponRecipeSystem(Forge forge, CraftingDataLoader craftingLoader, CraftingRecipeLoader recipeLoader)
    {
        this.forge = forge;
        forgeManager = forge.ForgeManager;
        this.craftingLoader = craftingLoader;
        this.recipeLoader = recipeLoader;

        UnlockedRecipeDict = new Dictionary<WeaponType, List<string>>();
        WeaponType[] weaponTypes = ForgeWeaponTypeMapping.ForgeWeaponTypeDict[forge.ForgeType];

        foreach (var type in weaponTypes)
        {
            UnlockedRecipeDict[type] = new List<string>();
        }

        InitDefaultRecipes();
    }

    public void InitDefaultRecipes()
    {
        foreach (var recipe in recipeLoader.RecipeList)
        {
            // NeedPoint 0 + 선행레시피 없음이면 최초 해금
            if (recipe.NeedPoint == 0 && string.IsNullOrEmpty(recipe.PrevRecipeKey))
            {
                if (!UnlockedRecipeDict[recipe.Type].Contains(recipe.Key))
                    UnlockedRecipeDict[recipe.Type].Add(recipe.Key);
            }
        }
    }

    public void UnlockRecipe(CraftingRecipeData recipeData)
    {
        if (forgeManager.UsePoint(recipeData.NeedPoint))
        {
            if (!UnlockedRecipeDict[recipeData.Type].Contains(recipeData.Key))
                UnlockedRecipeDict[recipeData.Type].Add(recipeData.Key);
        }
    }

    public void ResetRecipe()
    {
        foreach (var key in UnlockedRecipeDict.Keys)
        {
            UnlockedRecipeDict[key].Clear();
        }
    }


    public List<string> GetKeysByType(WeaponType type)
    {
        return UnlockedRecipeDict.ContainsKey(type) ? UnlockedRecipeDict[type] : new List<string>();
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

    public bool CheckUnlock(string key)
    {
        CraftingRecipeData data = recipeLoader.GetDataByKey(key);
        return UnlockedRecipeDict.ContainsKey(data.Type) && UnlockedRecipeDict[data.Type].Contains(data.Key);
    }

    public bool CanUnlock(CraftingRecipeData data)
    {
        if (CheckUnlock(data.Key)) return false;
        if (string.IsNullOrEmpty(data.PrevRecipeKey)) return true;
        CraftingRecipeData prevRecipe = recipeLoader.GetDataByKey(data.PrevRecipeKey);
        return UnlockedRecipeDict[prevRecipe.Type].Contains(prevRecipe.Key);
    }

    public List<ForgeRecipeSaveData> GetSaveData()
    {
        List<ForgeRecipeSaveData> unlockedKeys = new List<ForgeRecipeSaveData>();

        foreach (var key in UnlockedRecipeDict.Keys)
        {
            unlockedKeys.Add(new ForgeRecipeSaveData { WeaponType = key, RecipeKeys = UnlockedRecipeDict[key] });
        }

        return unlockedKeys;
    }

    public void LoadFormData(List<ForgeRecipeSaveData> recipes)
    {
        foreach (var recipe in recipes)
        {
            UnlockedRecipeDict[recipe.WeaponType] = recipe.RecipeKeys;
        }
    }
}
