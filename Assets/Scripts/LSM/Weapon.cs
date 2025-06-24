using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Weapon")]
public class Weapon : Item
{
    public int maxGemSlots = 2;
    public List<Gem> equippedGems = new List<Gem>();
}
