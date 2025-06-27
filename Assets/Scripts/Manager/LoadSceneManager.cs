using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Main,
    Dungeon,
    MiniGame
}
public static class SceneName
{
    public const string MainScene = "MainScene";
    public const string DungeonScene = "DungeonScene";
    public const string MiniGame = "MiniGame";

    public static string GetSceneByType(SceneType type)
    {
        return type switch
        {
           SceneType.Main => MainScene,
           SceneType.Dungeon => DungeonScene,
           SceneType.MiniGame => MiniGame,
            _ => string.Empty
        };
    }
}

public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private float fadeDuration = 1f;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 비동기 씬로딩
    /// </summary>
    /// <param name="type">Scene 타입</param>
    /// <param name="isAdditve">Additeve 모드 로딩 여부</param>
    public void LoadSceneAsync(SceneType type, bool isAdditve)
    {
        StartCoroutine(LoadSceneCoroutine(SceneName.GetSceneByType(type), isAdditve));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, bool isAdditve)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, isAdditve ? LoadSceneMode.Additive : LoadSceneMode.Single);

        // 로딩 스크린 페이드 아웃
        yield return StartCoroutine(FadeRoutine(fadeOutCurve, true));

        yield return new WaitUntil(() => asyncOperation.isDone);

        // 로딩 스크린 페이드 인
        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));
    }

    private IEnumerator FadeRoutine(AnimationCurve curve, bool blockRaycasts)
    {
        float time = 0f;
        loadingCanvas.blocksRaycasts = blockRaycasts;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            loadingCanvas.alpha = curve.Evaluate(t);
            time += Time.deltaTime;
            yield return null;
        }

        loadingCanvas.alpha = curve.Evaluate(1f);
        loadingCanvas.blocksRaycasts = blockRaycasts;
    }
}
