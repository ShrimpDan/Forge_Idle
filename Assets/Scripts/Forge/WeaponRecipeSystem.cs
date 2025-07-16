using System.Collections.Generic;

public class WeaponRecipeSystem
{
    private Forge forge;
    private CraftingDataLoader craftingLoader;

    public Dictionary<CustomerJob, List<string>> UnlockedRecipeDict { get; private set; }
    public int CurRecipePoint  { get; private set; }
    public int TotalRecipePoint { get; private set; }
    private int UsedPoint => TotalRecipePoint - CurRecipePoint;
    private int resetGold; 

    public WeaponRecipeSystem(Forge forge, CraftingDataLoader dataLoader)
    {
        this.forge = forge;
        craftingLoader = dataLoader;

        UnlockedRecipeDict = new Dictionary<CustomerJob, List<string>>
        {
            { CustomerJob.Woodcutter, new List<string>() },
            { CustomerJob.Farmer, new List<string>() },
            { CustomerJob.Miner, new List<string>() },
            { CustomerJob.Warrior, new List<string>() },
            { CustomerJob.Archer, new List<string>() },
            { CustomerJob.Tanker, new List<string>() },
            { CustomerJob.Assassin, new List<string>() },
        };

        AddBaseRecipe();
    }

    private void AddBaseRecipe()
    {
        UnlockedRecipeDict[CustomerJob.Woodcutter].Add(craftingLoader.CraftingList[0].ItemKey);
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
        if (forge.UseGold(resetGold * UsedPoint))
        {
            CurRecipePoint = TotalRecipePoint;

            foreach (var key in UnlockedRecipeDict.Keys)
            {
                UnlockedRecipeDict[key].Clear();
            }

            AddBaseRecipe();
        }
    }

    public void UnlockRecipe(CustomerJob job, string recipeKey)
    {
        UnlockedRecipeDict[job].Add(recipeKey);
    }

    public List<string> GetKeysByType(CustomerJob job)
    {
        return UnlockedRecipeDict[job];
    }

    public List<CraftingData> GetDatasByType(CustomerJob job)
    {
        if (UnlockedRecipeDict[job].Count > 0)
        {
            List<CraftingData> dataLists = new List<CraftingData>();

            foreach (var key in UnlockedRecipeDict[job])
            {
                dataLists.Add(craftingLoader.GetDataByKey(key));
            }

            return dataLists;
        }
        
        return null;
    }
}
