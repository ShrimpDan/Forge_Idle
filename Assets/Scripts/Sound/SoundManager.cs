using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드 매니저 싱글턴 클래스
/// BGM / SFX 재생, 볼륨 조절, 음소거, 페이드 기능 제공
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("사운드 데이터베이스")]
    [SerializeField] private SoundDatabase soundDatabase;

    private AudioSource bgmSource;
    private List<AudioSource> sfxSources = new();

    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private bool isBGMMuted = false;
    private bool isSFXMuted = false;

    private bool isFadingBGM = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        soundDatabase.Init();

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;

        bgmVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFX_KEY, 1f);
    }

    /// <summary>
    /// 사운드 이름으로 재생 (데이터베이스 기준)
    /// </summary>
    public void Play(string soundName)
    {
        var data = soundDatabase.GetSound(soundName);
        if (data == null || data.clip == null) return;

        switch (data.soundType)
        {
            case SoundType.BGM:
                PlayBGM(data.clip);
                break;
            case SoundType.SFX:
                PlaySFX(data.clip);
                break;
        }
    }

    /// <summary>
    /// BGM 재생 (중복 방지 포함)
    /// </summary>
    private void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = isBGMMuted ? 0 : bgmVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// SFX 재생 (AudioSource 풀링)
    /// </summary>
    private void PlaySFX(AudioClip clip)
    {
        AudioSource src = null;

        foreach (var s in sfxSources)
        {
            if (!s.isPlaying)
            {
                src = s;
                break;
            }
        }

        if (src == null)
        {
            src = gameObject.AddComponent<AudioSource>();
            sfxSources.Add(src);
        }

        src.clip = clip;
        src.loop = false;
        src.spatialBlend = 0f;
        src.volume = isSFXMuted ? 0 : sfxVolume;
        src.Play();
    }

    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = isBGMMuted ? 0 : bgmVolume;
    }

    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var src in sfxSources)
        {
            src.volume = isSFXMuted ? 0 : sfxVolume;
        }
    }

    /// <summary>
    /// BGM 음소거 설정
    /// </summary>
    public void MuteBGM(bool mute)
    {
        isBGMMuted = mute;
        bgmSource.volume = mute ? 0 : bgmVolume;
    }

    /// <summary>
    /// SFX 음소거 설정
    /// </summary>
    public void MuteSFX(bool mute)
    {
        isSFXMuted = mute;
        foreach (var src in sfxSources)
        {
            src.volume = mute ? 0 : sfxVolume;
        }
    }

    /// <summary>
    /// BGM을 페이드 아웃한 후, 새 BGM으로 페이드 인하여 자연스럽게 전환합니다.
    /// </summary>
    public void ChangeBGMWithFade(AudioClip newClip, float fadeDuration = 1.0f)
    {
        if (!isFadingBGM)
        {
            StartCoroutine(ChangeBGMWithFadeCoroutine(newClip, fadeDuration));
        }
    }

    /// <summary>
    /// BGM 페이드 전환 코루틴 (페이드 아웃 -> 교체 -> 페이드 인)
    /// </summary>
    private IEnumerator ChangeBGMWithFadeCoroutine(AudioClip newClip, float duration)
    {
        isFadingBGM = true;

        float startVolume = bgmSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, time / duration);
            yield return null;
        }

        bgmSource.volume = bgmVolume;
        isFadingBGM = false;
    }
}
