using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    private SoundManager soundManager;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] Button exitBtn;

    public const string BGM_KEY = "BGM_KEY";
    public const string SFX_KEY = "SFX_KEY";

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SettingWindow));

        if (soundManager == null)
        {
            soundManager = SoundManager.Instance;
            bgmSlider.onValueChanged.AddListener(ChangeBgmValue);
            sfxSlider.onValueChanged.AddListener(ChangeSfxValue);
        }
    }

    public override void Open()
    {
        base.Open();

        bgmSlider.value = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    public override void Close()
    {
        PlayerPrefs.SetFloat(BGM_KEY, bgmSlider.value);
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);

        base.Close();
    }

    private void ChangeBgmValue(float value)
    {
        soundManager.SetBGMVolume(value);
    }

    private void ChangeSfxValue(float value)
    {
        soundManager.SetSFXVolume(value);
    }
}
