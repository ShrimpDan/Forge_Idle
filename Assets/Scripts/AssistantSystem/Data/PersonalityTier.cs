using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 성격 데이터(PersonalityData)를 티어 등급별로 묶는 ScriptableObject입니다.
/// 예: Tier 1에는 완벽주의자, 성실한 등의 상위 성격들이 포함됩니다.
/// </summary>
[CreateAssetMenu(fileName = "PersonalityTier", menuName = "Trainee/PersonalityTier")]
public class PersonalityTier : ScriptableObject
{
    [Header("티어 정보")]

    [Tooltip("이 티어의 등급입니다. 숫자가 낮을수록 높은 등급입니다. 예: 1 = 최상위 티어, 5 = 최하위 티어")]
    [SerializeField] private int tierLevel;

    [Header("해당 티어의 성격 목록")]

    [Tooltip("이 티어에 속하는 PersonalityData 자산들을 집어서 넣으세요.")]
    [SerializeField] private List<PersonalityData> personalities;

    // 외부 접근용 Getter
    public int TierLevel => tierLevel;
    public List<PersonalityData> Personalities => personalities;
}