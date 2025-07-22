using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;

public class CustomerCollectionData
{

    public RegularCustomerData collectionData;
    public int visitedCount;
    public int maxVisitedCount;
    public bool isDicovered;
    public bool isEffectUnlocked;

    public float ProgressRatio => Mathf.Clamp01((float)visitedCount / maxVisitedCount);
    public float VisitorRateBonus => GetVisitorBonusByRarity();
    public float GoldBonus => GetVisitorBonusByGold();

    private float GetVisitorBonusByRarity()
    {
        switch (collectionData.rarity)
        {
            case CustomerRarity.Common:
                {
                    return 0.03f;
                }
            case CustomerRarity.Rare:
                {
                    return 0.05f;
                }
            case CustomerRarity.Epic:
                {
                    return 0.1f;
                }
            case CustomerRarity.Unique:
                {
                    return 0.15f;
                }
            case CustomerRarity.Legendary:
                {
                    return 0.2f;
                }
            default: return 0.0f;
        }
    }

    private float GetVisitorBonusByGold()
    {
        switch (collectionData.rarity)
        { 
        case CustomerRarity.Common:
            {
                return 0.5f;
            }
        case CustomerRarity.Rare:
            {
                return 0.05f;
            }
        case CustomerRarity.Epic:
            {
                return 0.1f;
            }
        case CustomerRarity.Unique:
            {
                return 0.15f;
            }
        case CustomerRarity.Legendary:
            {
                return 0.2f;
            }
        default: return 0.0f;
        }
        
    }

}
