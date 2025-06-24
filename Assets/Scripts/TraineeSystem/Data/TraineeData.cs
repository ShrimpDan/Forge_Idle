using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTrainee", menuName = "Trainee/TraineeData")]
public class TraineeData : ScriptableObject
{
    [Header("제자 기본 정보")]

    [Tooltip("제자의 이름입니다.")]
    [SerializeField] private string traineeName;

    [Tooltip("제자의 레벨입니다. 기본값은 1입니다  .")]
    [SerializeField] private int level = 1;

    [Tooltip("이 제자의 성격 데이터입니다. PersonalityData ScriptableObject를 참조합니다.")]
    [SerializeField] private PersonalityData personality;

    [Tooltip("이 제자가 특화된 분야입니다. 제작 / 강화 / 판매 중 하나입니다.")]
    [SerializeField] private SpecializationType specialization;

    [Header("특화 능력치 배율")]

    [Tooltip("특화에 따라 부여된 능력치 배율 목록입니다. 각 항목은 특정 효과와 배율을 포함합니다.")]
    [SerializeField] private List<AbilityMultiplier> multipliers = new List<AbilityMultiplier>();

    [Header("사용 상태")]

    [Tooltip("현재 제자가 착용 중인지 여부입니다.")]
    [SerializeField] private bool isEquipped = false;

    [Tooltip("현재 제자가 사용 가능 중인 상태인지 여부입니다.")]
    [SerializeField] private bool isUsable = false;


    // 외부 접근용 Getter

    public string TraineeName => traineeName;
    public int Level => level;
    public PersonalityData Personality => personality;
    public SpecializationType Specialization => specialization;
    public List<AbilityMultiplier> Multipliers => multipliers;

    public bool IsEquipped
    {
        get => isEquipped;
        set => isEquipped = value;
    }

    public bool IsUsable
    {
        get => isUsable;
        set => isUsable = value;
    }

    [System.Serializable]
    public class AbilityMultiplier
    {
        [Tooltip("적용되는 능력 이름입니다. 예: '판매 수익 증가'")]
        public string abilityName;

        [Tooltip("적용 배율입니다. 예: 1.5는 150% 효과")]
        public float multiplier;
    }
}
