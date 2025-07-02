using UnityEngine;

/// <summary>
/// 개별 이펙트 정보를 담는 ScriptableObject
/// 이펙트 이름, 타입, 프리팹 등을 정의합니다.
/// </summary>
[CreateAssetMenu(menuName = "Effects/EffectData", fileName = "NewEffectData")]
public class EffectData : ScriptableObject
{
    [Header("이펙트 이름 (파일명 자동 동기화)")]
    public string effectName;

    [Header("이펙트 타입 (UI, Combat 등)")]
    public EffectType effectType;

    [Header("연결된 파티클 프리팹")]
    public ParticleSystem prefab;

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 파일명과 effectName을 자동 동기화합니다.
    /// </summary>
    private void OnValidate()
    {
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

        if (!string.IsNullOrEmpty(fileName) && effectName != fileName)
        {
            effectName = fileName;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
