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
    }

    public void UnlockRecipe(CraftingRecipeData recipeData)
    {
        if (forgeManager.UsePoint(recipeData.NeedPoint))
        {
            UnlockedRecipeDict[recipeData.Type].Add(recipeData.Key);
            return;
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

    public bool CheckUnlock(string key)
    {
        CraftingRecipeData data = recipeLoader.GetDataByKey(key);

        if (UnlockedRecipeDict[data.Type].Contains(data.Key))
            return true;

        return false;
    }

    public bool CanUnlock(CraftingRecipeData data)
    {
        if (CheckUnlock(data.Key)) return false;

        // 선행 조건이 없는 경우 (첫 레시피)
        if (string.IsNullOrEmpty(data.PrevRecipeKey))
            return true;

        // 선행 레시피가 해금되었는지 확인
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
