using System.Collections.Generic;

public class WeaponRecipeSystem
{
    private Forge forge;
    private ForgeManager forgeManager;
    private CraftingDataLoader craftingLoader;
    private CraftingRecipeLoader recipeLoader;

    public Dictionary<WeaponType, List<string>> UnlockedRecipeDict { get; private set; } // 이거 무기타입별 해금된 제작법
    public int CurRecipePoint { get; private set; }
    public int TotalRecipePoint { get; private set; }
    private int UsedPoint => TotalRecipePoint - CurRecipePoint;
    private int resetGold;

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
        List<ForgeRecipeSaveData> saveDatas = new List<ForgeRecipeSaveData>();

        foreach (var key in UnlockedRecipeDict.Keys)
        {
            saveDatas.Add(new ForgeRecipeSaveData { WeaponType = key, recipeKeys = UnlockedRecipeDict[key] });
        }

        return saveDatas;
    }

    public void LoadFormData(List<ForgeRecipeSaveData> recipes)
    {
        foreach (var recipe in recipes)
        {
            UnlockedRecipeDict[recipe.WeaponType] = recipe.recipeKeys;
        }
    }
}
