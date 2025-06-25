using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자를 생성하고, 성격과 특화를 랜덤하게 부여한 후
/// 해당 데이터를 기반으로 게임 내 오브젝트를 소환하는 매니저 클래스입니다.
/// </summary>
public class TraineeManager : MonoBehaviour
{
    [Header("제자 소환 설정")]
    [SerializeField] private GameObject traineePrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    [Header("성격 데이터베이스")]
    public PersonalityTierDatabase personalityDatabase;

    [Header("소환 이펙트 매니저")]
    [SerializeField] private TraineeSummonManager summonEffectManager;

    private List<TraineeData> runtimeTrainees = new List<TraineeData>();

    private List<GameObject> activeTrainees = new List<GameObject>();

    private Dictionary<SpecializationType, int> specializationCounts = new();

    private PersonalityAssigner assigner;

    private void Awake()
    {
        assigner = new PersonalityAssigner(personalityDatabase);
    }

    public void RecruitAndSpawnTrainee()
    {
        TraineeData newTrainee = assigner.GenerateTrainee();

        if (newTrainee == null)
        {
            Debug.LogWarning("제자 생성 실패");
            return;
        }

        // 고유 이름 생성
        string traineeName = GenerateTraineeName(newTrainee.Specialization);
        newTrainee.SetName(traineeName);

        // 프리팹 생성
        GameObject obj = Instantiate(traineePrefab, spawnPosition, Quaternion.identity, spawnParent);
        obj.GetComponent<TraineeController>().Setup(newTrainee, this);

        // 리스트 등록
        runtimeTrainees.Add(newTrainee);
        activeTrainees.Add(obj);

        // 이펙트 출력
        summonEffectManager?.SummonEffectByTier(TraineeTier.Tier5);
    }

    /// <summary>
    /// 특정 제자를 삭제합니다 (데이터와 오브젝트 모두 제거).
    /// </summary>
    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        runtimeTrainees.Remove(data);
        activeTrainees.Remove(obj);

        if (specializationCounts.ContainsKey(data.Specialization) && specializationCounts[data.Specialization] > 1)
        {
            specializationCounts[data.Specialization]--;
        }

        Destroy(obj);
    }

    /// <summary>
    /// 특화별로 고유한 이름을 생성합니다.
    /// 예: 제자_Crafting 1, 제자_Selling 2 ...
    /// </summary>
    private string GenerateTraineeName(SpecializationType spec)
    {
        if (!specializationCounts.ContainsKey(spec))
            specializationCounts[spec] = 0;

        specializationCounts[spec]++;
        return $"제자_{spec} {specializationCounts[spec]}";
    }

    /// <summary>
    /// 제자를 착용 가능한 상태인지 확인한 후, 착용 처리합니다.
    /// - 사용 불가능하거나 이미 착용 중인 경우에는 착용되지 않습니다.
    /// </summary>
    public bool TryEquipTrainee(TraineeData data)
    {
        if (data.IsInUse)
        {
            Debug.Log("이 제자는 현재 사용할 수 없습니다.");
            return false;
        }

        if (data.IsEquipped)
        {
            Debug.Log("이미 착용 중입니다.");
            return false;
        }

        data.IsEquipped = true;
        data.IsInUse = true;
        return true;
    }

    public List<TraineeData> GetAllTrainees()
    {
        return runtimeTrainees;
    }
}

