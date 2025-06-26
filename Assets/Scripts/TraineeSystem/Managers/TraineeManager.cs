using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

/// <summary>
/// 제자를 생성하고, 게임 내에 소환하며, 관련 데이터를 관리하는 핵심 매니저 클래스입니다.
/// 제자 생성 요청을 처리하고, 씬에 오브젝트로 배치하며 UI 연출도 포함합니다.
/// </summary>
public class TraineeManager : MonoBehaviour
{
    [Header("제자 소환 설정")]
    [SerializeField] private GameObject traineePrefab;
    [SerializeField] private RectTransform cardRoot;

    [Header("성격 데이터베이스")]
    [SerializeField] private PersonalityTierDatabase personalityDatabase;

    [Header("런타임 관리")]
    private List<TraineeData> runtimeTrainees = new();
    private List<GameObject> activeTrainees = new();
    private List<TraineeController> activeControllers = new();
    private List<TraineeData> currentBatch = new();

    private bool isRecruiting = false;
    private bool isSkipping = false;
    private SpecializationType? currentDrawType = null;

    private TraineeFactory factory;

    private void Awake()
    {
        factory = new TraineeFactory(personalityDatabase);
    }

    public void OnClickRecruitRandomTrainee() => RecruitAndSpawnTrainee();
    public void OnClickRecruitCraftingTrainee() => RecruitAndSpawnFixed(SpecializationType.Crafting);
    public void OnClickRecruitEnhancingTrainee() => RecruitAndSpawnFixed(SpecializationType.Enhancing);
    public void OnClickRecruitSellingTrainee() => RecruitAndSpawnFixed(SpecializationType.Selling);

    public void OnClickRecruit10Random() => StartMultipleRecruit(10);
    public void OnClickRecruit10Crafting() => StartMultipleRecruit(10, SpecializationType.Crafting);
    public void OnClickRecruit10Enhancing() => StartMultipleRecruit(10, SpecializationType.Enhancing);
    public void OnClickRecruit10Selling() => StartMultipleRecruit(10, SpecializationType.Selling);

    public void RecruitAndSpawnTrainee()
    {
        if (isRecruiting) return;

        TraineeData data = factory.CreateRandomTrainee();
        if (data == null) return;

        Vector2 position = CalculateCardPositions(1)[0];
        SpawnTrainee(data, 0, position);
        ConfirmTrainee(data);
    }

    public void RecruitAndSpawnFixed(SpecializationType type)
    {
        if (isRecruiting) return;

        TraineeData data = factory.CreateFixedTrainee(type);
        if (data == null) return;

        Vector2 position = CalculateCardPositions(1)[0];
        SpawnTrainee(data, 0, position);
        ConfirmTrainee(data);
    }

    public void StartMultipleRecruit(int count, SpecializationType? fixedType = null)
    {
        if (isRecruiting) return;

        isRecruiting = true;
        currentBatch.Clear();
        isSkipping = false;
        currentDrawType = fixedType;

        factory.SetCanRecruit(true);
        StartCoroutine(RecruitMultipleCoroutine(count));
    }

    private IEnumerator RecruitMultipleCoroutine(int count)
    {
        List<Vector2> positions = CalculateCardPositions(count);

        for (int i = 0; i < count; i++)
        {
            if (isSkipping) break;

            TraineeData data = currentDrawType == null
                ? factory.CreateRandomTrainee()
                : factory.CreateFixedTrainee(currentDrawType.Value);

            if (data == null) break;

            SpawnTrainee(data, i, positions[i]);
            yield return new WaitUntil(() => factory.CanRecruit);
        }

        EndRecruiting();
    }

    private void SpawnTrainee(TraineeData data, int index, Vector2 targetPosition)
    {
        GameObject obj = Instantiate(traineePrefab, cardRoot);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 412);

        TraineeController controller = obj.GetComponent<TraineeController>();
        controller.Setup(data, factory, OnCardClicked);

        activeTrainees.Add(obj);
        activeControllers.Add(controller);

        rt.DOAnchorPos(targetPosition, 0.5f).SetEase(Ease.OutBack).SetDelay(index * 0.05f);
    }

    private List<Vector2> CalculateCardPositions(int count)
    {
        List<Vector2> positions = new();
        float spacingX = 212f;
        float startX = -spacingX * 2;
        float yTop = 707f;
        float yBottom = 412f;

        for (int i = 0; i < count; i++)
        {
            int row = i < 5 ? 0 : 1;
            int col = i % 5;
            float x = startX + col * spacingX;
            float y = row == 0 ? yTop : yBottom;
            positions.Add(new Vector2(x, y));
        }

        return positions;
    }

    private void ConfirmTrainee(TraineeData data)
    {
        currentBatch.Add(data);
        AddAndReindex(data);
    }

    private void OnSkipFromIndex(int currentIndex)
    {
        isSkipping = true;

        foreach (var obj in activeTrainees)
            Destroy(obj);

        activeTrainees.Clear();
        activeControllers.Clear();

        int alreadyCount = currentBatch.Count;
        int remaining = Mathf.Max(0, 10 - alreadyCount);

        for (int i = 0; i < remaining; i++)
        {
            TraineeData data = currentDrawType == null
                ? factory.CreateRandomTrainee(bypassRecruitCheck: true)
                : factory.CreateFixedTrainee(currentDrawType.Value, bypassRecruitCheck: true);

            if (data != null)
            {
                currentBatch.Add(data);
                AddAndReindex(data);
            }
        }

        factory.SetCanRecruit(true);
        StopAllCoroutines();
        EndRecruiting();
        DebugPrintAllTrainees();
    }

    private void AddAndReindex(TraineeData data)
    {
        runtimeTrainees.Add(data);
        var sameType = runtimeTrainees.FindAll(t => t.Specialization == data.Specialization);
        for (int i = 0; i < sameType.Count; i++)
            sameType[i].SpecializationIndex = i + 1;
    }

    private void OnCardClicked(TraineeController ctrl)
    {
        if (!ctrl.IsFlipped)
            ctrl.FlipImmediately();
    }

    public void OnClick_FlipAllCards()
    {
        foreach (var ctrl in activeControllers)
        {
            if (!ctrl.IsFlipped)
                ctrl.FlipImmediately();
        }
    }

    public void OnClick_ConfirmDraw()
    {
        if (!activeControllers.All(c => c.IsFlipped))
        {
            Debug.Log("모든 카드가 열려야 종료할 수 있습니다.");
            return;
        }

        foreach (var obj in activeTrainees)
            Destroy(obj);

        activeTrainees.Clear();
        activeControllers.Clear();
        EndRecruiting();
    }

    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        runtimeTrainees.Remove(data);
        activeTrainees.Remove(obj);
        Destroy(obj);

        var sameType = runtimeTrainees.FindAll(t => t.Specialization == data.Specialization);
        for (int i = 0; i < sameType.Count; i++)
            sameType[i].SpecializationIndex = i + 1;
    }

    public void DebugPrintAllTrainees()
    {
        Debug.Log($"[전체 제자 수]: {runtimeTrainees.Count}");
        for (int i = 0; i < runtimeTrainees.Count; i++)
        {
            var t = runtimeTrainees[i];
            Debug.Log($"[{i + 1}] 이름: {t.Name} / 특화: {t.Specialization} / 번호: {t.SpecializationIndex}");
        }
    }

    private void EndRecruiting()
    {
        isRecruiting = false;
    }

    public List<TraineeData> GetAllTrainees() => runtimeTrainees;

    public List<TraineeData> GetTraineesByType(SpecializationType type) =>
        runtimeTrainees.FindAll(t => t.Specialization == type);

    public List<TraineeData> GetLastDrawResult() => currentBatch;
}