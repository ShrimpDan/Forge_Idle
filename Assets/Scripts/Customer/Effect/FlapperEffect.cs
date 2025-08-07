using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapperEffect : MonoBehaviour
{
    [SerializeField] private GameObject impactEffectPrefabs;

    public void ShowImpactEffet()
    {
        Vector3 pos = transform.position + Vector3.up * 0.2f;
       
        
    }
}
