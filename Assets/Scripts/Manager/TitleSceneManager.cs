using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Animator titleAnim;
    [SerializeField] Button startButton;

    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        Application.targetFrameRate = 60;
        startButton.onClick.AddListener(ClickStartBtn);
    }

    private void Start()
    {
        SoundManager.Instance?.Play("MainBGM");

        if (titleAnim != null)
            titleAnim.SetTrigger("Start");
    }

    private void ClickStartBtn()
    {
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.Forge_Main);
    }
}
