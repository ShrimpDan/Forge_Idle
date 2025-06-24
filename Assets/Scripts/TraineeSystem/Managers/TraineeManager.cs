using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자를 생성하고, 성격과 특화를 랜덤하게 부여한 후
/// 해당 데이터를 기반으로 게임 내 오브젝트를 소환하는 매니저 클래스입니다.
/// </summary>
public class TraineeManager : MonoBehaviour
{
    [Header("제자 소환 설정")]

    [Tooltip("제자 프리팹입니다. UI 및 TraineeController가 포함된 GameObject를 할당합니다.")]
    [SerializeField] private GameObject traineePrefab;

    [Tooltip("소환된 제자들을 배치할 부모 오브젝트입니다.")]
    [SerializeField] private Transform spawnParent;

    [Tooltip("제자를 소환할 기준 월드 좌표입니다.")]
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    [Header("성격 데이터베이스")]

    [Tooltip("1~5티어 성격 정보를 담고 있는 ScriptableObject입니다.")]
    public PersonalityTierDatabase personalityDatabase;

    [Header("임시 저장용 제자 리스트 (SO)")]

    [Tooltip("생성된 제자 데이터를 임시로 저장하는 ScriptableObject입니다.")]
    [SerializeField] private TraineeListSO runtimeTraineeList;

    // 런타임에 소환된 제자 오브젝트들을 추적합니다.
    private List<GameObject> activeTrainees = new List<GameObject>();

    // 특화(SpecializationType)별 제자 수를 추적하여 이름을 고유하게 만듭니다.
    private Dictionary<SpecializationType, int> specializationCounts = new();

    // 성격/특화 랜덤 지정 기능을 수행하는 어사이너 인스턴스입니다.
    private PersonalityAssigner assigner;

    /// <summary>
    /// 게임 시작 시 PersonalityAssigner를 초기화합니다.
    /// </summary>
    private void Awake()
    {
        assigner = new PersonalityAssigner(personalityDatabase);
    }

    /// <summary>
    /// 제자를 랜덤으로 생성하고, 자동 정렬이 적용된 부모 오브젝트에 프리팹을 소환합니다.
    /// Grid Layout Group을 활용하므로 위치 계산은 필요 없습니다.
    /// </summary>
    public void RecruitAndSpawnTrainee()
    {
        // 1. 제자 데이터 생성 (랜덤 성격 및 특화 포함)
        TraineeData newTrainee = assigner.GenerateTrainee();

        // 2. 특화 기준 고유 이름 생성
        string traineeName = GenerateTraineeName(newTrainee.Specialization);

        // 3. private 필드인 이름에 직접 주입 (리플렉션)
        newTrainee.GetType().GetField("traineeName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(newTrainee, traineeName);

        // 4. 제자 프리팹을 소환 (부모 미지정 시 월드 위치 적용)
        GameObject obj = Instantiate(traineePrefab, spawnPosition, Quaternion.identity);
        obj.GetComponent<TraineeController>().Setup(newTrainee, this);

        // 5. 리스트에 데이터 등록 (SO)
        if (runtimeTraineeList != null)
            runtimeTraineeList.trainees.Add(newTrainee);
    }

    /// <summary>
    /// 특정 제자를 삭제합니다 (데이터와 오브젝트 모두 제거).
    /// </summary>
    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        // 1. 리스트에서 해당 제자 데이터 제거
        if (runtimeTraineeList != null)
            runtimeTraineeList.trainees.Remove(data);

        // 2. 특화별 이름 카운트 감소 (최소 1 이상일 때만)
        if (specializationCounts.ContainsKey(data.Specialization) && specializationCounts[data.Specialization] > 1)
        {
            specializationCounts[data.Specialization]--;
        }

        // 3. 게임 오브젝트 제거
        Destroy(obj);
    }

    /// <summary>
    /// 특화별로 고유한 이름을 생성합니다.
    /// 예: 제자_Crafting 1, 제자_Selling 2 ...
    /// </summary>
    private string GenerateTraineeName(SpecializationType spec)
    {
        int count = 1;

        foreach (var trainee in runtimeTraineeList.trainees)
        {
            if (trainee.Specialization == spec)
            {
                count++;
            }
        }

        return $"제자_{spec} {count}";
    }

    /// <summary>
    /// 제자를 착용 가능한 상태인지 확인한 후, 착용 처리합니다.
    /// - 사용 불가능하거나 이미 착용 중인 경우에는 착용되지 않습니다.
    /// </summary>
    public bool TryEquipTrainee(TraineeData data)
    {
        // 사용 불가능한 제자는 착용할 수 없음
        if (!data.IsUsable)
        {
            Debug.Log("이 제자는 현재 사용할 수 없습니다.");
            return false;
        }

        // 이미 착용 중인 제자는 다시 착용할 수 없음
        if (data.IsEquipped)
        {
            Debug.Log("이미 착용 중입니다.");
            return false;
        }

        // 조건을 모두 통과했으면 착용 처리
        data.IsEquipped = true;
        return true;
    }
}

