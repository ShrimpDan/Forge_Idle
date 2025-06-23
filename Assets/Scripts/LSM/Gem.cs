using UnityEngine;

[CreateAssetMenu(fileName = "Gem", menuName = "Game/Gem")]
public class Gem : Item
{
    public StatType statType;
    public int bonusValue;
}
