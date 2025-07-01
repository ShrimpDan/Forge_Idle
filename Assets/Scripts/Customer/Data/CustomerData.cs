using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerType
{ 
    Normal,
    Regualr,
    Nuisance
}


public enum CustomerJob
{
    Woodcutter,
    Farmer,
    Miner,
    Warrior,
    Archer,
    Assassin

}

public enum CustomerRarity
{ 
    Common,
    Rare,
    Epic,
    Unique,
    Legendary
}

[CreateAssetMenu(fileName = "CustomerData",menuName ="Data/CustomerData", order =0)]

public class CustomerData : ScriptableObject
{
    [SerializeField] private CustomerJob job;
    [SerializeField] private CustomerType type;
    public int buyCount;
    public float frequency;
    public float buyingTime;
    public float moveSpeed;


    public CustomerType Type => type;
    public CustomerJob Job => job;
 
}

[CreateAssetMenu(fileName = "CustomerData", menuName = "Data/RegualrCustomerData", order = 0)]
public class RegualrCustomerData : ScriptableObject
{
    [Header("기본 정보")]
    public CustomerData customerData;
    

    public string customerName;
    public string customerInfo;
    public Sprite Icon;
    [HideInInspector]public bool isDiscovered;
    public CustomerRarity rarity;

    public CustomerJob Job => customerData != null ? customerData.Job : CustomerJob.Farmer;

    

    //직업은 커스텀 데이터로 가지고 있으니까 

}

[CreateAssetMenu(fileName = "CollectionBookData",menuName = "Data/CollectionBookData", order = 2)]
public class CollectionBookData : ScriptableObject
{
    public List<RegualrCustomerData> regularCustomers = new();
}
