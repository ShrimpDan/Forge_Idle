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

    private TraineeFactory factory;

    private void Awake()
    {
        factory = new TraineeFactory(personalityDatabase);
    }

    /// <summary>
    /// 무작위 특화의 제자를 소환합니다 (랜덤 버튼).
    /// </summary>
    public void OnClickRecruitRandomTrainee() => RecruitAndSpawnRandom();

    /// <summary>
    /// 제작 특화 제자를 소환합니다.
    /// </summary>
    public void OnClickRecruitCraftingTrainee() => RecruitAndSpawnFixed(SpecializationType.Crafting);

    /// <summary>
    /// 강화 특화 제자를 소환합니다.
    /// </summary>
    public void OnClickRecruitEnhancingTrainee() => RecruitAndSpawnFixed(SpecializationType.Enhancing);

    /// <summary>
    /// 판매 특화 제자를 소환합니다.
    /// </summary>
    public void OnClickRecruitSellingTrainee() => RecruitAndSpawnFixed(SpecializationType.Selling);

    /// <summary>
    /// 랜덤 제자 데이터를 생성하고 게임 내에 배치합니다.
    /// </summary>
    public void RecruitAndSpawnRandom()
    {
        TraineeData data = factory.CreateRandomTrainee();
        if (data == null)
        {
            return;
        }
        SpawnTrainee(data);
    }

    /// <summary>
    /// 특정 특화 제자를 생성하고 게임 내에 배치합니다.
    /// </summary>
    public void RecruitAndSpawnFixed(SpecializationType type)
    {
        TraineeData data = factory.CreateFixedTrainee(type);
        if (data == null)
        {
            return;
        }

        SpawnTrainee(data);
    }

    /// <summary>
    /// 제자 오브젝트를 씬에 생성하고 초기화합니다.
    /// </summary>
    private void SpawnTrainee(TraineeData data)
    {
        if (traineePrefab == null)
        {
            Debug.LogError("제자 프리팹이 연결되어 있지 않습니다.");
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

        controller.Setup(data, factory);
        controller.PlaySpawnEffect();

        runtimeTrainees.Add(data);
        activeTrainees.Add(obj);
    }


    /// <summary>
    /// 제자를 삭제하고 관리 목록에서도 제거합니다.
    /// </summary>
    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        runtimeTrainees.Remove(data);
        activeTrainees.Remove(obj);
        Destroy(obj);
    }

    /// <summary>
    /// 현재 존재하는 모든 제자 데이터를 반환합니다.
    /// </summary>
    public List<TraineeData> GetAllTrainees() => runtimeTrainees;

    /// <summary>
    /// 특정 특화의 제자 목록을 반환합니다.
    /// </summary>
    public List<TraineeData> GetTraineesByType(SpecializationType type) =>
        runtimeTrainees.FindAll(t => t.Specialization == type);
}
