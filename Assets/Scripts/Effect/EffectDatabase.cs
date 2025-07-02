using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 등록된 이펙트들을 관리하는 데이터베이스
/// 런타임에 Dictionary로 빠르게 조회합니다.
/// </summary>
[CreateAssetMenu(menuName = "Effects/EffectDatabase", fileName = "EffectDatabase")]
public class EffectDatabase : ScriptableObject
{
    public List<EffectData> effects;

    private Dictionary<string, EffectData> effectMap;

    public void Init()
    {
        effectMap = new Dictionary<string, EffectData>();

        foreach (var effect in effects)
        {
            if (!effectMap.ContainsKey(effect.effectName))
                effectMap.Add(effect.effectName, effect);
        }
    }

    public EffectData GetEffect(string name)
    {
        effectMap.TryGetValue(name, out var data);
        return data;
    }
}
