/// <summary>
/// 제자의 특화 유형을 나타내는 열거형입니다.
/// </summary>
public enum SpecializationType
{
    Crafting,
    Enhancing,
    Selling
}

public static class AssistantStatNames
{
    public const string IncreaseCraftSpeed = "제작 속도 증가";
    public const string IncreaseAdvancedCraftChance = "고급 제작 확률 증가";
    public const string IncreaseEnhanceChance = "강화 확률 증가";
    public const string DecreaseBreakChance = "파괴 확률 감소";
    public const string DecreaseEnhanceCost = "강화 비용 감소";
    public const string IncreaseSellPrice = "판매 수익 증가";
    public const string IncreaseCustomerCount = "손님 수 증가";
}