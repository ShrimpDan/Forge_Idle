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
    [Header("Loading Animation")]
    [SerializeField] private Animator loadingAnim;
    private int loadingHash = Animator.StringToHash("IsLoading");

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Camera Reference")]
    [SerializeField] private GameObject mainCameraObject;

    private SoundManager soundManager;
    private SceneType _lastActiveSceneType = SceneType.Forge_Main;

    protected override void Awake()
    {
        base.Awake();
        soundManager = SoundManager.Instance;
    }

    // --- SCENE LOAD ---
    public void LoadSceneAsync(SceneType type, bool isAdditive = false)
    {
        loadingAnim.SetBool(loadingHash, true);

        string bgmName = GetBGMNameBySceneType(type);
        SoundManager.Instance?.StopBGM();
        SoundManager.Instance?.Play(bgmName);

        _lastActiveSceneType = type;

        StartCoroutine(LoadSceneCoroutine(SceneName.GetSceneByType(type), isAdditive));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, bool isAdditive)
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.alpha = 1f;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncOperation.isDone);

        if (isAdditive)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid())
                SceneManager.SetActiveScene(loadedScene);
        }

        EnsureMainCameraActive();
        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));
    }

    // --- SCENE UNLOAD (직접 remainSceneType 넘길 때) ---
    public void UnLoadScene(SceneType type, SceneType remainSceneType = SceneType.Forge_Main)
    {
        loadingAnim.SetBool(loadingHash, true);
        StartCoroutine(UnLoadSceneCoroutine(SceneName.GetSceneByType(type), remainSceneType));
    }

    private IEnumerator UnLoadSceneCoroutine(string sceneName, SceneType remainSceneType)
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.alpha = 1f;
        loadingAnim.SetBool(loadingHash, true);

        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncOperation.isDone);

        if (!SceneCameraState.IsMineSceneActive)
            yield return StartCoroutine(EnsureMainCameraReallyActive());

        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));

        // Fade가 끝난 뒤에만 BGM 전환, 로딩창 숨김 완료
        string remainBgmName = GetBGMNameBySceneType(remainSceneType);
        SoundManager.Instance?.StopBGM();
        SoundManager.Instance?.Play(remainBgmName);

        _lastActiveSceneType = remainSceneType;
    }

    // --- SCENE UNLOAD (콜백 버전) ---
    public void UnLoadScene(SceneType type, Action onComplete)
    {
        loadingAnim.SetBool(loadingHash, true);
        StartCoroutine(UnLoadSceneCoroutine_Compat(SceneName.GetSceneByType(type), onComplete));
    }

    private IEnumerator UnLoadSceneCoroutine_Compat(string sceneName, Action onComplete)
    {
        loadingCanvas.blocksRaycasts = true;
        loadingCanvas.alpha = 1f;
        loadingAnim.SetBool(loadingHash, true);

        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncOperation.isDone);

        if (!SceneCameraState.IsMineSceneActive)
            yield return StartCoroutine(EnsureMainCameraReallyActive());

        yield return StartCoroutine(FadeRoutine(fadeInCurve, false));

        // Fade 끝나고 UI 숨김 완료
        onComplete?.Invoke();

        SceneType remainType = SceneCameraState.IsMineSceneActive ? SceneType.MineScene : SceneType.Forge_Main;
        string remainBgmName = GetBGMNameBySceneType(remainType);
        SoundManager.Instance?.StopBGM();
        SoundManager.Instance?.Play(remainBgmName);

        _lastActiveSceneType = remainType;
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

        loadingCanvas.alpha = 0f; // Fade 끝난 후 로딩창 숨김
        loadingCanvas.blocksRaycasts = false;
        loadingAnim.SetBool(loadingHash, false);
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

    public void SetMainCamera(GameObject cameraObject)
    {
        mainCameraObject = cameraObject;
    }

    private string GetBGMNameBySceneType(SceneType type)
    {
        return type switch
        {
            SceneType.Dungeon => "DungeonBGM",
            SceneType.MiniGame => "MainBGM",
            SceneType.MineScene => "MineBGM",
            SceneType.Forge_Weapon => "MainBGM",
            SceneType.Forge_Armor => "ArmorBGM",
            SceneType.Forge_Magic => "MagicBGM",
            SceneType.Forge_Main => "MainBGM",
            _ => "MainBGM"
        };
    }
}
