using UnityEngine;
using TMPro; // TextMeshPro 사용 시 반드시 필요

public class TraineeController : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text tierText;

    private TraineeData data;

    /// <summary>
    /// ScriptableObject 데이터를 받아 제자 정보를 UI에 표시합니다.
    /// </summary>
    public void Setup(TraineeData traineeData)
    {
        data = traineeData;

        if (nameText != null)
            nameText.text = data.TraineeName;

        if (tierText != null)
            tierText.text = $"{data.Personality.Tier}티어 / {data.Personality.PersonalityName}";
    }

    // 마우스로 클릭 시 정보 출력
    public void OnClick_ShowDetails()
    {
        Debug.Log($"[정보 출력] 이름: {data.TraineeName} / 성격: {data.Personality.PersonalityName}");

        foreach (var mul in data.Multipliers)
        {
            Debug.Log($"- {mul.abilityName} x{mul.multiplier}");
        }
    }
}
