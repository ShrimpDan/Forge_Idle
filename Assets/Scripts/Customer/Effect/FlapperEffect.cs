using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapperEffect : MonoBehaviour
{
    [SerializeField] private GameObject impactEffectPrefabs;

    public void ShowImpactEffet()
    {
        Vector3 pos = transform.position + Vector3.down * 0.2f;
        Instantiate(impactEffectPrefabs, pos, Quaternion.identity);
        
    }
}
