using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingRecipeData
{
    public string Key;
    public WeaponType Type;
    public int NeedPoint;
    public string PrevRecipeKey;
}

public class CraftingRecipeLoader
{
    public List<CraftingRecipeData> RecipeList { get; private set; }
    public Dictionary<string, CraftingRecipeData> RecipeDict { get; private set; }

    public CraftingRecipeLoader(string path = "Data/crafting_recipe_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("CraftingRecipe 데이터가 존재하지 않습니다.");
            return;
        }

        RecipeList = JsonUtility.FromJson<Wrapper>(json.text).Items;
        RecipeDict = new Dictionary<string, CraftingRecipeData>();

        foreach (var recipe in RecipeList)
        {
            RecipeDict[recipe.Key] = recipe;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<CraftingRecipeData> Items;
    }

    public CraftingRecipeData GetDataByKey(string key)
    {
        RecipeDict.TryGetValue(key, out CraftingRecipeData data);
        return data;
    }
}
