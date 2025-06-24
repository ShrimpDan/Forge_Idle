using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDataManger : MonoBehaviour
{
    public static TestDataManger Instance { get; private set; }

    public ItemDataLoader ItemLoader { get; private set; }

    void Awake()
    {
        Instance = this;
        ItemLoader = new ItemDataLoader();
    }
}
