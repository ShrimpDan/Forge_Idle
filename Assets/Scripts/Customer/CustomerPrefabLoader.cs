using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPrefabLoader
{

    private Dictionary<(CustomerJob, CustomerType), Customer> normalPrefabs = new();
    private Dictionary<(CustomerJob, CustomerRarity), Customer> regularPrefabs = new();
    private Customer nuisancePrefab;



    public void LoadAll()
    {
        LoadNormalPrefabs();
        LoadRegularPrefabs();
        LoadNuisancePrefab();
    }


    private void LoadNormalPrefabs()
    {
        foreach (CustomerJob job in Enum.GetValues(typeof(CustomerJob)))
        {
            string path = $"Prefabs/Customer/Normal/{job}";
            var prefabs = Resources.Load<Customer>(path);
            if (prefabs != null)
            {
                normalPrefabs[(job, CustomerType.Normal)] = prefabs;
            }
        }
    
    }

    private void LoadRegularPrefabs()
    {
        foreach (CustomerJob job in Enum.GetValues(typeof(CustomerJob)))
        {
            foreach (CustomerRarity rarity in Enum.GetValues(typeof(CustomerRarity)))
            {
                string path = $"Prefabs/Customer/Regular/{job}/{rarity}";
                var prefabs = Resources.Load<Customer>(path);
                if (prefabs != null)
                {
                    regularPrefabs[(job, rarity)] = prefabs;
                }

                
            }
        }

    }

    private void LoadNuisancePrefab()       // ★ 진상은 1개만
    {
        nuisancePrefab = Resources.Load<Customer>("Prefabs/Customer/Nuisance/NuisanceCustomer");
        if (nuisancePrefab == null)
        { 
            Debug.LogWarning("진상 프리팹 없음");
        }
    }


    public Customer GetNormal(CustomerJob job) => normalPrefabs.TryGetValue((job, CustomerType.Normal), out var prefab) ? prefab : null;

    public Customer GetRegular(CustomerJob job, CustomerRarity rarity) => regularPrefabs.TryGetValue((job, rarity), out var prefab) ? prefab : null;

    public Customer GetNuisance() => nuisancePrefab;
}
