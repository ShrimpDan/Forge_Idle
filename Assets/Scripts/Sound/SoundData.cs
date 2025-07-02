using UnityEngine;

/// <summary>
/// 개별 사운드 정보를 담는 ScriptableObject
/// 이름, 사운드 타입, 클립을 보관합니다.
/// </summary>
[CreateAssetMenu(menuName = "Audio/SoundData", fileName = "NewSoundData")]
public class SoundData : ScriptableObject
{
    public string soundName;
    public SoundType soundType;
    public AudioClip clip;
}