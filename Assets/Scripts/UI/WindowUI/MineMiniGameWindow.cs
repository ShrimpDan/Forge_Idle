using System;
using UnityEngine;
using UnityEngine.UI;
public class MineMiniGameWindow : MonoBehaviour
{
    private Button btn;
    private DateTime lastClearDate;
    private const string LastClearKey = "MiniGameClear";

    private void Awake()
    {
        btn = GetComponent<Button>();
        LoadClearDate();
    }

    private void OnEnable()
    {
        UpdateButtonState();
    }

    public void OnClickMiniGame()
    {
       
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.MiniGame, true);
        

    }

    private void UpdateButtonState()
    {
        // 하루가 지나면 초기화
        if (TimeManager.Instance.HasOneDayPassed(lastClearDate))
        {
            lastClearDate = DateTime.MinValue;
            SaveClearDate();
        }

      
        btn.interactable = !HasClearedToday();
    }

    public void MarkCleared()
    {

        lastClearDate = TimeManager.Instance.Now();
        SaveClearDate();

        btn.interactable = false;
    }


    private bool HasClearedToday()
    {
        return lastClearDate.Date == TimeManager.Instance.Now().Date;
    }

    private void LoadClearDate()
    {
        string saved = PlayerPrefs.GetString(LastClearKey, "");
        if (!string.IsNullOrEmpty(saved))
            lastClearDate = DateTime.Parse(saved);
        else
            lastClearDate = DateTime.MinValue;
    }

    private void SaveClearDate()
    {
        PlayerPrefs.SetString(LastClearKey, lastClearDate.ToString());
    }
}
