using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MineMiniGameWindow : MonoBehaviour
{
    public void OnClickMiniGame()
    {
        SceneManager.LoadScene("MiniGame", LoadSceneMode.Additive);
    }

 
}
