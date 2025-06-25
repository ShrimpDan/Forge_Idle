using UnityEngine;

public class TraineeSummonManager : MonoBehaviour
{
    [Header("이펙트 프리팹 (등급별)")]
    [SerializeField] private GameObject tier1EffectPrefab;
    [SerializeField] private GameObject tier2EffectPrefab;
    [SerializeField] private GameObject tier3EffectPrefab;
    [SerializeField] private GameObject tier4EffectPrefab;
    [SerializeField] private GameObject tier5EffectPrefab;

    [Header("이펙트 위치")]
    [SerializeField] private Transform summonPoint;

    public void SummonEffectByTier(TraineeTier tier)
    {
        GameObject effectPrefab = GetEffectByTier(tier);
        if (effectPrefab == null) return;

        GameObject effectInstance = Instantiate(effectPrefab);
        var effect = effectInstance.GetComponent<TraineeSummonEffect>();
        if (effect != null)
        {
            effect.PlayEffect(summonPoint.position);
        }
    }

    private GameObject GetEffectByTier(TraineeTier tier)
    {
        switch (tier)
        {
            case TraineeTier.Tier1: return tier1EffectPrefab;
            case TraineeTier.Tier2: return tier2EffectPrefab;
            case TraineeTier.Tier3: return tier3EffectPrefab;
            case TraineeTier.Tier4: return tier4EffectPrefab;
            case TraineeTier.Tier5: return tier5EffectPrefab;
            default: return null;
        }
    }
}
