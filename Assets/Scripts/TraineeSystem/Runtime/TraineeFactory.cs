using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자 생성과 이름 자동 부여를 담당하는 팩토리 클래스입니다.
/// 특화 랜덤/고정 제자 모두 생성 가능하며, 이름은 특화별로 카운팅됩니다.
/// </summary>
public class TraineeFactory
{
    [Header("특화별 이름 카운트")]
    private readonly Dictionary<SpecializationType, int> specializationCounts = new();

    private readonly PersonalityAssigner assigner;
    private bool canRecruit = true;

    public TraineeFactory(PersonalityTierDatabase database)
    {
        assigner = new PersonalityAssigner(database);
    }

    /// <summary>
    /// 현재 제자 생성이 가능한 상태인지 반환합니다.
    /// </summary>
    public bool CanRecruit => canRecruit;

    /// <summary>
    /// 제자 생성 가능 상태를 설정합니다.
    /// </summary>
    public void SetCanRecruit(bool value)
    {
        canRecruit = value;
    }

    /// <summary>
    /// 무작위 특화의 제자를 생성합니다.
    /// </summary>
    public TraineeData CreateRandomTrainee(bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;

        canRecruit = false;
        var data = assigner.GenerateTrainee();
        if (data != null)
            AssignInfo(data);

        return data;
    }

    /// <summary>
    /// 특정 특화의 제자를 생성합니다.
    /// </summary>
    public TraineeData CreateFixedTrainee(SpecializationType type, bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;

        canRecruit = false;
        var data = assigner.GenerateTrainee(type);
        if (data != null)
            AssignInfo(data);

        return data;
    }

    /// <summary>
    /// 특화별로 고유 이름과 인덱스를 부여합니다.
    /// </summary>
    private void AssignInfo(TraineeData data)
    {
        var spec = data.Specialization;

        if (!specializationCounts.ContainsKey(spec))
            specializationCounts[spec] = 0;

        specializationCounts[spec]++;
        data.SpecializationIndex = specializationCounts[spec];
        data.SetName($"제자_{spec} {data.SpecializationIndex}");
    }
}
