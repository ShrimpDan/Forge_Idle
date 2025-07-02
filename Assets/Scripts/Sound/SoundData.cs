using UnityEngine;

/// <summary>
/// 개별 사운드 정보를 담는 ScriptableObject
/// 사운드 이름, 클립, 타입(BGM/SFX) 등을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "NewSound", menuName = "Sound/New Sound Data")]
public class SoundData : ScriptableObject
{
    [Tooltip("사운드 이름 (파일명 자동 동기화)")]
    public string soundName;

    [Tooltip("재생할 오디오 클립")]
    public AudioClip clip;

    [Tooltip("사운드 타입 (BGM 또는 SFX)")]
    public SoundType soundType;

    /// <summary>
    /// 에디터에서 파일 이름과 soundName을 자동으로 동기화합니다.
    /// </summary>
    private void OnValidate()
    {
#if UNITY_EDITOR
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

        if (!string.IsNullOrEmpty(fileName) && soundName != fileName)
        {
            soundName = fileName;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}