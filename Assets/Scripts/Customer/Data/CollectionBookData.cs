using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectionBookData", menuName = "Data/CollectionBookData", order = 2)]
public class CollectionBookData : ScriptableObject
{
    public List<RegularCustomerData> regularCustomers = new();
}
