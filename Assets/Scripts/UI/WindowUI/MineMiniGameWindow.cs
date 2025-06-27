using UnityEngine;

public class MineMiniGameWindow : MonoBehaviour
{
    public void OnClickMiniGame()
    {
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.MiniGame, true);
    }
}
