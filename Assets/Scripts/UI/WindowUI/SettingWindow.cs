using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    private SoundManager soundManager;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] Button exitBtn;

    

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

        bgmSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_KEY, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.SFX_KEY, 1f);
    }

    public override void Close()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.BGM_KEY, bgmSlider.value);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.SFX_KEY, sfxSlider.value);

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
