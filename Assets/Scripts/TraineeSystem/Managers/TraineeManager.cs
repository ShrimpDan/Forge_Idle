using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraineeManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("카드 프리팹")]
    [SerializeField] private GameObject largeTraineeCardPrefab;
    [SerializeField] private GameObject miniTraineeCardPrefab;

    [Header("버튼 프리팹")]
    [SerializeField] private GameObject confirmAllButtonPrefab;

    [Header("카드 출력 위치")]
    [SerializeField] private Transform singleDrawParent;
    [SerializeField] private Transform multiDrawParent;

    [Header("애니메이터")]
    [SerializeField] private TraineeCardLayoutAnimator layoutAnimator;

    [Header("Draw 컨트롤러")]
    [SerializeField] private TraineeDrawController drawController;

    [Header("UI 배경")]
    [SerializeField] private GameObject backgroundPanel;

    private TraineeFactory factory;
    private TraineeCardSpawner spawner;
    private TraineeInventory inventory = new();
    public TraineeInventory TraineeInventory => inventory;
    public TraineeCardSpawner Spawner => spawner;

    private List<TraineeData> currentBatch = new();
    private bool canRecruit = true;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        factory = new TraineeFactory(gameManager.DataManager);
        spawner = new TraineeCardSpawner(
            largeTraineeCardPrefab,
            miniTraineeCardPrefab,
            singleDrawParent,
            multiDrawParent,
            factory,
            drawController
        );

        drawController.Init(factory, spawner);
        drawController.OnTraineeConfirmed += ConfirmTrainee;
        drawController.OnRecruitingFinished += () => {
            canRecruit = true;

            if (backgroundPanel != null)
                backgroundPanel.SetActive(false);
        };
    }

    // 단일 뽑기 처리 (외부에서 호출)
    public void RecruitSingle(SpecializationType? type = null)
    {
        HandleSingleRecruit(() =>
            type == null
                ? factory.CreateRandomTrainee()
                : factory.CreateFixedTrainee(type.Value)
        );
    }

    // 다중 뽑기 처리
    public void RecruitMultiple(int count, SpecializationType? type = null)
    {
        if (!canRecruit) return;
        canRecruit = false;
        drawController.StartMultipleRecruit(count, type);
    }

    // 내부 처리 함수
    private void HandleSingleRecruit(System.Func<TraineeData> recruitFunc)
    {
        if (!canRecruit) return;
        canRecruit = false;

        StartCoroutine(SingleRecruitFlow(recruitFunc));
    }

    private IEnumerator SingleRecruitFlow(System.Func<TraineeData> recruitFunc)
    {
        if (backgroundPanel != null)
            backgroundPanel.SetActive(true);

        yield return null;

        var data = recruitFunc.Invoke();
        if (data == null) yield break;

        var obj = spawner.SpawnLargeCard(data);
        ConfirmTrainee(data);
    }

    // 외부에서 제자 등록
    public void ConfirmTrainee(TraineeData data)
    {
        currentBatch.Add(data);
        inventory.Add(data);
    }

    // 외부에서 제자 제거
    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        inventory.Remove(data);
        Destroy(obj);
        canRecruit = true;

        if (backgroundPanel != null)
            backgroundPanel.SetActive(false);
    }

    // 디버그 / 데이터 접근
    public void DebugPrintAllTrainees() => inventory.DebugPrint();
    public List<TraineeData> GetAllTrainees() => inventory.GetAll();
    public List<TraineeData> GetTraineesByType(SpecializationType type) => inventory.GetBySpecialization(type);
    public List<TraineeData> GetLastDrawResult() => currentBatch;
}
