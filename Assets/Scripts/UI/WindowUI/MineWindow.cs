using UnityEngine;

public class MineWindow : MonoBehaviour
{
    public void OnClickEnterMine()
    {
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.MineScene, true);
    }
}