using UnityEngine;

public class MineSceneManager : MonoBehaviour
{
    [Header("메인 오브젝트")]
    public GameObject mineDetailMap;

    [Header("마인 프리팹들")]
    public GameObject[] minePrefabs; // 0=Copper, 1=Iron, 2=Gold, 3=Gem

    [Header("카메라 리밋")]
    public CameraLimit[] cameraLimits; // [0]=DetailMap, [1]=Copper, [2]=Iron, [3]=Gold, [4]=Gem

    [Header("카메라 이동 컴포넌트")]
    public CameraTouchDrag cameraTouchDrag;

    private int currentMineIdx = 0;

    private void Start()
    {
        ShowMineDetailMap();
    }

    public void ShowMineDetailMap()
    {
        mineDetailMap.SetActive(true);
        for (int i = 0; i < minePrefabs.Length; i++)
            minePrefabs[i].SetActive(false);

        if (cameraTouchDrag != null && cameraLimits.Length > 0)
            cameraTouchDrag.SetCameraLimit(cameraLimits[0]);
        currentMineIdx = 0;
    }

    public void ShowMine(int mineIndex)
    {
        mineDetailMap.SetActive(false);
        for (int i = 0; i < minePrefabs.Length; i++)
            minePrefabs[i].SetActive(i == mineIndex);

        // cameraLimits[1] = 0번 마인, [2] = 1번 마인...
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
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.Main);
    }
}
