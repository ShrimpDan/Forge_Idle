using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자를 생성하고, 게임 내에 소환하며, 관련 데이터를 관리하는 핵심 매니저 클래스입니다.
/// 제자 생성 요청을 처리하고, 씬에 오브젝트로 배치합니다.
/// </summary>
public class TraineeManager : MonoBehaviour
{
    [Header("제자 소환 설정")]
    [SerializeField] private GameObject traineePrefab;

    [Header("성격 데이터베이스")]
    [SerializeField] private PersonalityTierDatabase personalityDatabase;

    [Header("런타임 관리")]
    private List<TraineeData> runtimeTrainees = new();
    private List<GameObject> activeTrainees = new();
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

        SpawnTrainee(data, 0);
        ConfirmTrainee(data);
    }

    public void RecruitAndSpawnFixed(SpecializationType type)
    {
        if (isRecruiting) return;

        TraineeData data = factory.CreateFixedTrainee(type);
        if (data == null) return;

        SpawnTrainee(data, 0);
        ConfirmTrainee(data);
    }

    private void SpawnTrainee(TraineeData data, int index)
    {
        if (traineePrefab == null)
        {
            Debug.LogError("제자 프리팹이 없습니다.");
            return;
        }

        GameObject obj = Instantiate(traineePrefab);
        TraineeController controller = obj.GetComponent<TraineeController>();

        if (controller == null)
        {
            Debug.LogError("TraineeController가 없습니다.");
            Destroy(obj);
            return;
        }

        controller.Setup(data, factory, index, OnSkipFromIndex, ConfirmTrainee);
        controller.PlaySpawnEffect();

        activeTrainees.Add(obj);
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
        for (int i = 0; i < count; i++)
        {
            if (isSkipping) break;

            TraineeData data = currentDrawType == null
                ? factory.CreateRandomTrainee()
                : factory.CreateFixedTrainee(currentDrawType.Value);

            if (data == null) break;

            SpawnTrainee(data, i);

            yield return new WaitUntil(() => factory.CanRecruit);
        }

        EndRecruiting();
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
        {
            Destroy(obj);
        }
        activeTrainees.Clear();

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
