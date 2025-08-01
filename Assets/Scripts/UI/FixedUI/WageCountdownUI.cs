using System.Collections;
using TMPro;
using UnityEngine;

public class WageCountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float intervalSeconds = 60f;

    private float timer;
    private Coroutine countdownRoutine;

    private void Start()
    {
        StartCountdown();
    }

    public void StartCountdown()
    {
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        countdownRoutine = StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (true)
        {
            timer = intervalSeconds;

            while (timer > 0f)
            {
                UpdateText(Mathf.CeilToInt(timer));
                yield return new WaitForSeconds(1f);
                timer -= 1f;
            }

            UpdateText(0);

            GameManager.Instance?.DebugWageTick();
            yield return null;
        }
    }

    private void UpdateText(int seconds)
    {
        if (countdownText != null)
            countdownText.text = $"시급 : {seconds}s";
    }

    public void ResetTimer()
    {
        StartCountdown();
    }
}
