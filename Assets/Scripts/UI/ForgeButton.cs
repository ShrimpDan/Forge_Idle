using UnityEngine;
using UnityEngine.UI;

public class ForgeButton : MonoBehaviour
{
    [SerializeField] private SceneType sceneType;
    private Button myBtn;

    void Awake()
    {
        myBtn = GetComponent<Button>();
        myBtn.onClick.AddListener(EnterForge);
    }

    private void EnterForge()
    {
        LoadSceneManager.Instance.LoadSceneAsync(sceneType, true);
    }
}
