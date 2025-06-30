using UnityEngine;

/// <summary>
/// PersonalityData는 제자의 성격을 정의하는 ScriptableObject입니다.
/// 각 성격은 고유의 이름, 티어(1~5), 그리고 제작/강화/판매 특화 능력치 배율을 가집니다.
/// 이를 통해 제자 육성 시 성격에 따라 능력치가 다르게 적용됩니다.
/// </summary>
[CreateAssetMenu(fileName = "NewPersonality", menuName = "Trainee/Personality")]
public class PersonalityDataSO : ScriptableObject
{
    [Header("성격 기본 정보")]

    [Tooltip("성격의 명칭입니다.")]
    [SerializeField] private string personalityName;

    [Tooltip("성격의 티어 (1~5) 낮을수록 상위 티어입니다.")]
    [SerializeField] private int tier;

    [Header("특화 능력치 배율")]

    [Tooltip("제작 능력치에 곱해지는 배율입니다.")]
    [SerializeField] private float craftingMultiplier = 1f;

    [Tooltip("강화 능력치에 곱해지는 배율입니다.")]
    [SerializeField] private float enhancingMultiplier = 1f;

    [Tooltip("판매 능력치에 곱해지는 배율입니다.")]
    [SerializeField] private float sellingMultiplier = 1f;

    // 외부 접근용 Getter
    public string PersonalityName => personalityName;
    public int Tier => tier;
    public float CraftingMultiplier => craftingMultiplier;
    public float EnhancingMultiplier => enhancingMultiplier;
    public float SellingMultiplier => sellingMultiplier;
}