using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이펙트를 실행하는 전역 매니저 (싱글턴)
/// EffectDatabase를 기반으로 이름으로 이펙트를 재생합니다.
/// </summary>
public class EffectManager_SRMR : MonoBehaviour
{
    public static EffectManager_SRMR Instance { get; private set; }

    [Header("이펙트 데이터베이스")]
    [SerializeField] private EffectDatabase effectDatabase;

    [Header("UI용 카메라 (UI 이펙트용)")]
    [SerializeField] private Camera uiCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        effectDatabase.Init();
    }

    /// <summary>
    /// 월드 위치에 이펙트 재생
    /// </summary>
    public void PlayEffect(string effectName, Vector3 position, Quaternion? rotation = null, Transform parent = null)
    {
        var data = effectDatabase.GetEffect(effectName);
        if (data == null || data.prefab == null) return;

        var instance = Instantiate(data.prefab, position, rotation ?? Quaternion.identity, parent);
        instance.Play();

        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
    }

    /// <summary>
    /// UI 이펙트 재생 (스크린 좌표 기반)
    /// </summary>
    public void PlayUIEffect(string effectName, Vector2 screenPosition)
    {
        if (uiCamera == null) return;

        var data = effectDatabase.GetEffect(effectName);
        if (data == null || data.prefab == null) return;

        Vector3 worldPos = uiCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
        PlayEffect(effectName, worldPos);
    }
}
