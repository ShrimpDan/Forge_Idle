using UnityEngine;

public class MineSceneManager : MonoBehaviour
{
    [Header("메인 오브젝트 (상세맵)")]
    public GameObject mineDetailMap;

    [Header("마인 프리팹들 (순서 맞게)")]
    public GameObject[] minePrefabs; // 0=Copper, 1=Iron, 2=Gold, 3=Gem

    [Header("카메라 리밋 (0=디테일맵, 1~4=각 마인)")]
    public CameraLimit[] cameraLimits; // [0]=DetailMap, [1]=Copper, [2]=Iron, [3]=Gold, [4]=Gem

    [Header("카메라 이동 컴포넌트")]
    public CameraTouchDrag cameraTouchDrag;

    private int currentMineIdx = 0;

    private GameObject mainSceneCam;
    private GameObject mineSceneCam;

    private void Start()
    {
        ShowMineDetailMap();
    }

    private void OnEnable()
    {
        mainSceneCam = GameObject.Find("MainCamera");
        if (mainSceneCam != null) mainSceneCam.SetActive(false);

        mineSceneCam = GameObject.Find("MapCamera");
        if (mineSceneCam != null) mineSceneCam.SetActive(true);
    }

    private void OnDisable()
    {
        if (mainSceneCam != null) mainSceneCam.SetActive(true);
        if (mineSceneCam != null) mineSceneCam.SetActive(false);
    }

    // 모든 맵, 마인 프리팹 비활성화
    private void SetAllInactive()
    {
        if (mineDetailMap != null)
            mineDetailMap.SetActive(false);

        if (minePrefabs != null)
        {
            foreach (var prefab in minePrefabs)
                if (prefab != null)
                    prefab.SetActive(false);
        }
    }

    // 상세맵 보기 (마인 프리팹 전부 끔)
    public void ShowMineDetailMap()
    {
        SetAllInactive();
        if (mineDetailMap != null)
            mineDetailMap.SetActive(true);

        if (cameraTouchDrag != null && cameraLimits.Length > 0)
            cameraTouchDrag.SetCameraLimit(cameraLimits[0]);

        currentMineIdx = 0;
    }

    public void ShowMine(int mineIndex)
    {
        SetAllInactive();
        if (minePrefabs != null && mineIndex >= 0 && mineIndex < minePrefabs.Length)
            minePrefabs[mineIndex].SetActive(true);

        if (cameraTouchDrag != null && cameraLimits.Length > mineIndex + 1)
            cameraTouchDrag.SetCameraLimit(cameraLimits[mineIndex + 1]);

        currentMineIdx = mineIndex + 1;
    }

    public void OnCopperBronzeMineBtn() => ShowMine(0);
    public void OnIronSilveMineBtn() => ShowMine(1);
    public void OnGoldmithrilMineBtn() => ShowMine(2);
    public void OnGemMineBtn() => ShowMine(3);
    public void OnBackToDetailMap() => ShowMineDetailMap();

    public void OnGoToMainScene()
    {
        LoadSceneManager.Instance.UnLoadScene(SceneType.MineScene);
    }
}
