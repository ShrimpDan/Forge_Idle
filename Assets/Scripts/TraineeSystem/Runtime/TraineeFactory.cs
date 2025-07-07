using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// 제자 생성과 이름 자동 부여를 담당하는 팩토리 클래스입니다.
/// 특화 랜덤/고정 제자 모두 생성 가능하며, 이름은 특화별로 카운팅됩니다.
/// </summary>
public class TraineeFactory
{
    private readonly Dictionary<SpecializationType, int> specializationCounts = new();
    private readonly AssistantDataLoader assistantLoader;
    private readonly PersonalityDataLoader personalityLoader;
    private readonly SpecializationDataLoader specializationLoader;
    private readonly System.Random rng = new();

    private bool canRecruit = true;

    public TraineeFactory(DataManager dataManager)
    {
        assistantLoader = dataManager.AssistantLoader;
        personalityLoader = dataManager.PersonalityLoader;
        specializationLoader = dataManager.SpecializationLoader;
    }

    public bool CanRecruit => canRecruit;

    public void SetCanRecruit(bool value)
    {
        canRecruit = value;
    }

    public TraineeData CreateRandomTrainee(bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        var candidates = assistantLoader.ItemsList.FindAll(t => GetTier(t.grade) >= 2);
        if (candidates.Count == 0) return null;

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateTraineeFromData(selected);
    }

    public TraineeData CreateFixedTrainee(SpecializationType type, bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        var candidates = assistantLoader.ItemsList.FindAll(t => GetTier(t.grade) >= 2 &&
                                                                specializationLoader.GetByKey(t.specializationKey)?.specializationType == type);
        if (candidates.Count == 0) return null;

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateTraineeFromData(selected);
    }

    public List<TraineeData> CreateMultiple(int count, SpecializationType? fixedType = null)
    {
        var results = new List<TraineeData>();
        for (int i = 0; i < count; i++)
        {
            var data = fixedType == null
                ? CreateRandomTrainee(true)
                : CreateFixedTrainee(fixedType.Value, true);

            if (data != null)
                results.Add(data);
        }
        return results;
    }

    public void ResetRecruitLock()
    {
        canRecruit = true;
    }

    private TraineeData CreateTraineeFromData(AssistantData assistant)
    {
        int tier = GetTier(assistant.grade);

        PersonalityData personalityData = personalityLoader.GetByKey(assistant.personalityKey);
        SpecializationData specializationData = specializationLoader.GetByKey(assistant.specializationKey);

        float m = 1f;
        switch (specializationData.specializationType)
        {
            case SpecializationType.Crafting:
                m = personalityData.craftingMultiplier;
                break;
            case SpecializationType.Enhancing:
                m = personalityData.enhancingMultiplier;
                break;
            case SpecializationType.Selling:
                m = personalityData.sellingMultiplier;
                break;
        }

        var multipliers = new List<TraineeData.AbilityMultiplier>();

        for (int i = 0; i < specializationData.statNames.Count; i++)
        {
            multipliers.Add(new TraineeData.AbilityMultiplier(
                abilityName: specializationData.statNames[i],
                multiplier: specializationData.statValues[i] * m));
        }

        TraineeData traineeData = new TraineeData(
            name: assistant.Name,
            personality: personalityData,
            specialization: specializationData.specializationType,
            multipliers: multipliers
        );

        AssignInfo(traineeData);

        return traineeData;
    }

    private void AssignInfo(TraineeData data)
    {
        var spec = data.Specialization;
        if (!specializationCounts.ContainsKey(spec))
            specializationCounts[spec] = 0;

        specializationCounts[spec]++;
        data.SpecializationIndex = specializationCounts[spec];
    }

    private int GetTier(string grade)
    {
        return grade switch
        {
            "UR" => 1,
            "SSR" => 2,
            "SR" => 3,
            "R" => 4,
            "N" => 5,
            _ => 5
        };
    }
}
