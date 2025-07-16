using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Main,
    Dungeon,
    MiniGame,
    MineScene,
    Forge_Weapon,
    Forge_Armor,
    Forge_Magic
}

public static class SceneName
{
    public const string MainScene = "MainScene";
    public const string DungeonScene = "DungeonScene";
    public const string MiniGame = "MiniGame";
    public const string MineScene = "MineScene";
    public const string Forge_Weapon = "Forge_Weapon";
    public const string Forge_Armor = "Forge_Armor";
    public const string Forge_Magic = "Forge_Magic";

    public static string GetSceneByType(SceneType type)
    {
        return type switch
        {
            SceneType.Main => MainScene,
            SceneType.Dungeon => DungeonScene,
            SceneType.MiniGame => MiniGame,
            SceneType.MineScene => MineScene,
            SceneType.Forge_Weapon => Forge_Weapon,
            SceneType.Forge_Armor => Forge_Armor,
            SceneType.Forge_Magic => Forge_Magic,
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
    public void LoadSceneAsync(SceneType type, bool isAdditve = false)
    {
        StartCoroutine(LoadSceneCoroutine(SceneName.GetSceneByType(type), isAdditve));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, bool isAdditive)
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.alpha = 1;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncOperation.isDone);

        // 씬 최상위로
        if (isAdditive)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid())
                SceneManager.SetActiveScene(loadedScene);
        }

        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));
    }


    public void UnLoadScene(SceneType type)
    {
        StartCoroutine(UnLoadSceneCoroutine(SceneName.GetSceneByType(type)));
    }

    IEnumerator UnLoadSceneCoroutine(string sceneName)
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.alpha = 1;

        // 비동기 씬 로딩
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
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
