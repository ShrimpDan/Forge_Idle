using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;



public enum SceneType
{
    Dungeon,
    MiniGame,
    MineScene,
    Forge_Weapon,
    Forge_Armor,
    Forge_Magic,
    Forge_Main
}

public static class SceneName
{
    public const string DungeonScene = "DungeonScene";
    public const string MiniGame = "MiniGame";
    public const string MineScene = "MineScene";
    public const string Forge_Weapon = "Forge_Weapon";
    public const string Forge_Armor = "Forge_Armor";
    public const string Forge_Magic = "Forge_Magic";
    public const string Forge_Main = "Forge_Main";

    

    public static string GetSceneByType(SceneType type)
    {
        return type switch
        {
            SceneType.Dungeon => DungeonScene,
            SceneType.MiniGame => MiniGame,
            SceneType.MineScene => MineScene,
            SceneType.Forge_Weapon => Forge_Weapon,
            SceneType.Forge_Armor => Forge_Armor,
            SceneType.Forge_Magic => Forge_Magic,
            SceneType.Forge_Main => Forge_Main,
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

    [Header("Camera Reference")]
    [SerializeField] private GameObject mainCameraObject;

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

        if (isAdditive)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid())
                SceneManager.SetActiveScene(loadedScene);
        }

        // MainCamera 활성화 체크
        EnsureMainCameraActive();

        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));
    }

    public void UnLoadScene(SceneType type)
    {
        StartCoroutine(UnLoadSceneCoroutine(SceneName.GetSceneByType(type)));
    }

    // 오버로드: 콜백 추가 버전
    public void UnLoadScene(SceneType type, Action onComplete)
    {
        StartCoroutine(UnLoadSceneCoroutine(SceneName.GetSceneByType(type), onComplete));
    }

    IEnumerator UnLoadSceneCoroutine(string sceneName)
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.alpha = 1;

        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncOperation.isDone);

        if (!SceneCameraState.IsMineSceneActive)
            yield return StartCoroutine(EnsureMainCameraReallyActive());

        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));
    }

    private IEnumerator UnLoadSceneCoroutine(string sceneName, Action onComplete)
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;

        if (!SceneCameraState.IsMineSceneActive)
            yield return StartCoroutine(EnsureMainCameraReallyActive());

        onComplete?.Invoke();
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

    private void EnsureMainCameraActive()
    {
        if (!SceneCameraState.IsMineSceneActive)
        {
            if (mainCameraObject != null && !mainCameraObject.activeSelf)
            {
                mainCameraObject.SetActive(true);
                var cam = mainCameraObject.GetComponent<Camera>();
                if (cam != null && !cam.enabled)
                {
                    cam.enabled = true;
                    Debug.Log("[LoadSceneManager] MainCamera 컴포넌트까지 강제 활성화!");
                }
                Debug.Log("[LoadSceneManager] MainCamera 강제로 활성화!");
            }
            else if (mainCameraObject == null)
            {
                Debug.LogWarning("[LoadSceneManager] 인스펙터에 MainCamera 오브젝트가 할당 안됨!");
            }
        }
        else
        {
            Debug.Log("[LoadSceneManager] Mine씬 활성 중. MainCamera 유지하지 않음.");
        }
    }

    private IEnumerator EnsureMainCameraReallyActive()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            if (mainCameraObject != null)
            {
                if (!mainCameraObject.activeSelf)
                    mainCameraObject.SetActive(true);

                var cam = mainCameraObject.GetComponent<Camera>();
                if (cam != null && !cam.enabled)
                    cam.enabled = true;

                Debug.Log("[LoadSceneManager] MainCamera와 Camera 컴포넌트 강제 활성화!");
                break;
            }
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        if (mainCameraObject == null)
            Debug.LogError("[LoadSceneManager] 인스펙터에 MainCamera 오브젝트가 할당 안됨!");
    }

}
