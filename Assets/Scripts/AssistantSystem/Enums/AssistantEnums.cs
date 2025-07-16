/// <summary>
/// 제자의 특화 유형을 나타내는 열거형입니다.
/// </summary>
public enum SpecializationType
{
    Crafting,
    Mining,
    Selling
}

public static class AssistantStatNames
{
    public const string IncreaseCraftSpeed = "제작 속도 증가";
    public const string IncreaseAdvancedCraftChance = "고급 제작 확률 증가";
    public const string IncreaseAutoCraftSpeed = "자동 제작 속도 증가";
    public const string IncreaseGreatSuccessChance = "대성공 확률 증가";

    public const string IncreaseMiningYieldPerMinute = "분 당 자원 채굴량 증가";
    public const string IncreaseMaxMiningCapacity = "최대 자원량 증가";

    public const string IncreaseSellPrice = "판매 수익 증가";
    public const string IncreaseCustomerCount = "손님 수 증가";
    public const string IncreaseAutoCustomerRepelChance = "자동 진상 퇴치 확률";
}