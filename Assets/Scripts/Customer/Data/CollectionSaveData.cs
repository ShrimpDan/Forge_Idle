using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectionSaveData
{

    public string collectionkey;
    public int visitedCount;
    public bool isDiscovered;
    public bool isEffectUnlocked;
}

[Serializable]
public class CollectionSaveDataList
{
    public List<CollectionSaveData> collectionDataList = new List<CollectionSaveData>();
    
}


