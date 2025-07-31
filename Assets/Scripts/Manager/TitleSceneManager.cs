using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] Animator titleAnim;
    [SerializeField] Button startButton;

    private void Awake()
    {
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
