using UnityEngine;
using TMPro;

/// <summary>
/// TraineeData를 받아 UI에 표시하고, 상호작용(삭제/정보 출력)을 처리하는 컨트롤러 클래스입니다.
/// </summary>
public class TraineeController : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text detailText;

    private TraineeData data;
    private TraineeManager manager;

    /// <summary>
    /// 제자 데이터를 받아 UI를 초기화합니다.
    /// </summary>
    public void Setup(TraineeData traineeData, TraineeManager traineeManager)
    {
        data = traineeData;
        manager = traineeManager;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (nameText != null)
            nameText.text = data.Name;

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
            multiplierLines += $"\n- {mul.AbilityName} x{mul.Multiplier:F2}";
        }

        return $"{header}{multiplierLines}";
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
