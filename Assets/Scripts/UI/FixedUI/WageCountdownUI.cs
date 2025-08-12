using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WageCountdown : MonoBehaviour
{
    [SerializeField] private Image wageFill;
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
                UpdateBar(Mathf.CeilToInt(timer));
                yield return new WaitForSeconds(1f);
                timer -= 1f;
            }

            UpdateBar(0);

            GameManager.Instance?.DebugWageTick();
            yield return null;
        }
    }

    private void UpdateBar(int seconds)
    {
        wageFill.fillAmount = seconds / intervalSeconds;
        wageFill.color = seconds <= 10f ? Color.red : Color.white;
    }

    public void ResetTimer()
    {
        StartCountdown();
    }
}
