using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드 데이터를 모두 모아 관리하는 데이터베이스
/// 이름으로 빠르게 사운드를 찾을 수 있도록 Dictionary 구성입니다.
/// </summary>
[CreateAssetMenu(menuName = "Audio/SoundDatabase", fileName = "SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    public List<SoundData> sounds;

    private Dictionary<string, SoundData> soundMap;

    /// <summary>
    /// Dictionary 초기화 (매니저에서 Awake 등에서 호출)
    /// </summary>
    public void Init()
    {
        soundMap = new Dictionary<string, SoundData>();

        foreach (var sound in sounds)
        {
            if (!soundMap.ContainsKey(sound.soundName))
                soundMap.Add(sound.soundName, sound);
        }
    }

    /// <summary>
    /// 사운드 이름으로 검색하여 SoundData 반환
    /// </summary>
    public SoundData GetSound(string name)
    {
        soundMap.TryGetValue(name, out var sound);
        return sound;
    }
}
