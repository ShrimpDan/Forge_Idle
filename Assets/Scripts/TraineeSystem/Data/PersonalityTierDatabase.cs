using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// PersonalityTierDatabase는 전체 성격 티어 데이터를 통합 관리하는 ScriptableObject입니다.
/// 각 PersonalityTier에는 동일한 등급의 성격들이 포함되어 있으며,
/// 이를 통해 제자 생성 시 티어별 성격을 확률적으로 선택할 수 있습니다.
/// </summary>
[CreateAssetMenu(fileName = "PersonalityTierDatabase", menuName = "Trainee/PersonalityTierDatabase")]
public class PersonalityTierDatabase : ScriptableObject
{
    [Tooltip("전체 티어 목록입니다. 각 티어에는 해당 등급의 성격들이 포함됩니다.")]
    public List<PersonalityTier> tiers;
}