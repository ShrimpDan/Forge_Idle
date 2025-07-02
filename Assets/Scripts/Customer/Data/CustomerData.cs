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
    Tanker,
    Assassin

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



