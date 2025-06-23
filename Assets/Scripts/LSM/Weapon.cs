using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Game/Weapon")]
public class Weapon : Item
{
    public int level = 1;
    public int baseAttack;
    public List<Gem> equippedGems = new List<Gem>();
    public int maxGemSlots = 2;

    public int GetTotalAttack()
    {
        int gemBonus = 0;
        foreach (var gem in equippedGems)
        {
            if (gem != null && gem.statType == StatType.Attack)
                gemBonus += gem.bonusValue;
        }
        return baseAttack + gemBonus;
    }
}
