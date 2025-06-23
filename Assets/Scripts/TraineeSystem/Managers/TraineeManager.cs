using UnityEngine;

/// <summary>
/// 제자를 생성하고, 성격과 특화를 랜덤하게 부여한 후
/// 해당 데이터를 기반으로 게임 내 오브젝트를 소환하는 매니저 클래스입니다.
/// </summary>
public class TraineeManager : MonoBehaviour
{
    [Header("성격 티어 데이터베이스")]
    [Tooltip("티어별 성격 정보가 담긴 ScriptableObject")]
    [SerializeField] private PersonalityTierDatabase personalityDB;

    [Header("프리팹 및 생성 위치")]
    [Tooltip("소환할 제자 프리팹")]
    [SerializeField] private GameObject traineePrefab;

    [Tooltip("프리팹을 소환할 위치")]
    [SerializeField] private Transform spawnPoint;

    // 제자 이름 카운터 (임시 이름용)
    private static int traineeCount = 1;

    /// <summary>
    /// 버튼 클릭 시 제자를 생성하고 게임 내 오브젝트로 소환합니다.
    /// </summary>
    public void OnClick_CreateTrainee()
    {
        // 1. 제자 데이터 생성 (ScriptableObject)
        TraineeData trainee = CreateTrainee();

        // 2. 프리팹 인스턴스 생성
        GameObject go = Instantiate(traineePrefab, spawnPoint.position, Quaternion.identity);

        // 3. 프리팹의 컨트롤러에 ScriptableObject 전달
        TraineeController controller = go.GetComponent<TraineeController>();
        if (controller != null)
        {
            controller.Setup(trainee);
        }

        // 4. 디버그 출력
        Debug.Log($"제자 생성됨! 이름: {trainee.TraineeName} / 티어: {trainee.Personality.Tier} / 성격: {trainee.Personality.PersonalityName} / 특화: {trainee.Specialization}");
    }

    /// <summary>
    /// 새로운 제자를 ScriptableObject 형태로 임시 생성합니다.
    /// </summary>
    public TraineeData CreateTrainee()
    {
        // 성격과 특화 랜덤 할당
        PersonalityAssigner assigner = new PersonalityAssigner(personalityDB);
        TraineeData trainee = assigner.GenerateTrainee();

        // 임시 이름 부여 (예: 제자1, 제자2 ...)
        trainee.GetType().GetField("traineeName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(trainee, $"제자{traineeCount++}");

        return trainee;
    }
}
