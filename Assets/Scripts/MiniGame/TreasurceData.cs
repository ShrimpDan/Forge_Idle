using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GemType
{ 
    Gem,
    Dia
}

[CreateAssetMenu(fileName = "TreasurceData", menuName ="Data/Treasurse")]
public class TreasureData : ScriptableObject
{
    public int id;
    public string Name;
    public Vector2Int[] Shape;
    public Sprite[] pratSprite;
    public Sprite rewardImage;
    public GemType gemType;
    
}
