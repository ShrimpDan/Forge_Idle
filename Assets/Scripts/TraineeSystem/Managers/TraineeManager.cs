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

    private TraineeFactory factory;
    private TraineeCardSpawner spawner;
    private TraineeInventory inventory = new();

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
        drawController.OnRecruitingFinished += () => canRecruit = true;
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

        var data = recruitFunc.Invoke();
        if (data == null) return;

        spawner.SpawnLargeCard(data);
        ConfirmTrainee(data);
        canRecruit = true;
    }

    // 데이터 저장
    public void ConfirmTrainee(TraineeData data)
    {
        currentBatch.Add(data);
        inventory.Add(data);
    }

    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        inventory.Remove(data);
        Destroy(obj);
    }

    // 디버그 / 데이터 접근
    public void DebugPrintAllTrainees() => inventory.DebugPrint();
    public List<TraineeData> GetAllTrainees() => inventory.GetAll();
    public List<TraineeData> GetTraineesByType(SpecializationType type) => inventory.GetBySpecialization(type);
    public List<TraineeData> GetLastDrawResult() => currentBatch;
}
