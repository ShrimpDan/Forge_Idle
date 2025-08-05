using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Animator titleAnim;
    [SerializeField] Button startButton;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        startButton.onClick.AddListener(ClickStartBtn);
    }

    private void Start()
    {
        if (titleAnim != null)
            titleAnim.SetTrigger("Start");
    }

    private void ClickStartBtn()
    {
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.Forge_Main);
    }
}
