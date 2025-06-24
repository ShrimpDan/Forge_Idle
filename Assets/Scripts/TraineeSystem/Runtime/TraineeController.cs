using UnityEngine;
using TMPro;

/// <summary>
/// TraineeData를 받아 UI에 표시하고, 상호작용(삭제/정보 출력)을 처리하는 컨트롤러 클래스입니다.
/// </summary>
public class TraineeController : MonoBehaviour
{
    [Header("UI 요소")]

    [Tooltip("제자의 이름을 표시할 TextMeshPro 텍스트입니다.")]
    [SerializeField] private TMP_Text nameText;

    [Tooltip("제자의 상세 정보를 표시할 TextMeshPro 텍스트입니다.")]
    [SerializeField] private TMP_Text detailText;

    // 현재 제자의 데이터 참조
    private TraineeData data;

    // 제자를 관리하는 매니저 참조 (삭제 시 호출용)
    private TraineeManager manager;

    /// <summary>
    /// 제자 데이터를 받아 UI를 초기화합니다.
    /// </summary>
    public void Setup(TraineeData traineeData, TraineeManager traineeManager)
    {
        data = traineeData;
        manager = traineeManager;

        // 이름 출력
        if (nameText != null)
            nameText.text = data.TraineeName;

        // 상세 정보 출력
        if (detailText != null)
            detailText.text = GetFormattedDetailText();
    }

    /// <summary>
    /// 제자의 상세 정보를 텍스트로 구성하여 반환합니다.
    /// </summary>
    private string GetFormattedDetailText()
    {
        string tierText = $"(티어 {data.Personality.Tier})";
        string header = $"특화: {data.Specialization}\n성격: {data.Personality.PersonalityName} {tierText}";

        string multiplierLines = "";
        foreach (var mul in data.Multipliers)
        {
            multiplierLines += $"\n- {mul.abilityName} x{mul.multiplier:F2}";
        }

        return $"{header}{multiplierLines}";
    }

    /// <summary>
    /// 콘솔에 제자 정보를 출력합니다. - 디버그용 , 추후 삭제
    /// </summary>
    public void OnClick_ShowDetails()
    {
        Debug.Log($"[정보 출력] 이름: {data.TraineeName} / 성격: {data.Personality.PersonalityName}");

        foreach (var mul in data.Multipliers)
        {
            Debug.Log($"- {mul.abilityName}: {mul.multiplier}");
        }
    }

    /// <summary>
    /// 제자 삭제 버튼을 눌렀을 때 호출되는 함수입니다.
    /// 매니저에 삭제 요청을 전달합니다.
    /// 이후에 제자 방출 시스템에 사용할 기능입니다.
    /// </summary>
    public void OnClick_DeleteSelf()
    {
        if (manager != null)
            manager.RemoveTrainee(gameObject, data);
    }
}
