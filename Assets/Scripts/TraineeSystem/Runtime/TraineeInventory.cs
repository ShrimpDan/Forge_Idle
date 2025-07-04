using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자 데이터의 리스트를 관리하고, 추가/삭제/조회/정렬 기능을 담당합니다.
/// </summary>
public class TraineeInventory
{
    private List<TraineeData> traineeList = new();

    /// <summary>
    /// 제자를 리스트에 추가하고 특화 인덱스를 갱신합니다.
    /// </summary>
    public void Add(TraineeData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[TraineeInventory] Null 데이터를 추가하려고 시도했습니다.");
            return;
        }

        traineeList.Add(data);
        ReindexSpecialization(data.Specialization);
    }

    /// <summary>
    /// 제자를 리스트에서 제거하고 특화 인덱스를 다시 계산합니다.
    /// </summary>
    public void Remove(TraineeData data)
    {
        if (!traineeList.Remove(data))
        {
            Debug.LogWarning("[TraineeInventory] 리스트에서 제거 실패: 해당 제자가 존재하지 않습니다.");
            return;
        }

        ReindexSpecialization(data.Specialization);
    }

    /// <summary>
    /// 전체 제자 리스트를 반환합니다.
    /// </summary>
    public List<TraineeData> GetAll()
    {
        return new List<TraineeData>(traineeList); // 보호용 복사본 반환
    }

    /// <summary>
    /// 특정 특화 타입의 제자들만 반환합니다.
    /// </summary>
    public List<TraineeData> GetBySpecialization(SpecializationType type)
    {
        return traineeList.FindAll(t => t.Specialization == type);
    }

    /// <summary>
    /// 장착 중인 제자들을 반환합니다.
    /// </summary>
    public List<TraineeData> GetEquippedTrainees()
    {
        return traineeList.FindAll(t => t.IsEquipped);
    }

    /// <summary>
    /// 디버그 용도로 제자 리스트 전체를 출력합니다.
    /// </summary>
    public void DebugPrint()
    {
        Debug.Log($"[전체 제자 수]: {traineeList.Count}");
        for (int i = 0; i < traineeList.Count; i++)
        {
            var t = traineeList[i];
            Debug.Log($"[{i + 1}] {ToStringTrainee(t)}");
        }
    }

    private string ToStringTrainee(TraineeData data)
    {
        return $"이름: {data.Name} / 특화: {data.Specialization} / 인덱스: {data.SpecializationIndex} / 레벨: {data.Level}";
    }

    /// <summary>
    /// 해당 특화 타입의 제자 인덱스를 재정렬합니다.
    /// </summary>
    private void ReindexSpecialization(SpecializationType type)
    {
        var sameType = traineeList.FindAll(t => t.Specialization == type);
        for (int i = 0; i < sameType.Count; i++)
        {
            sameType[i].SpecializationIndex = i + 1;
        }
    }

    /// <summary>
    /// 티어 우선, 특화 후순 정렬
    /// </summary>
    public void SortByTierThenSpecialization()
    {
        traineeList.Sort((a, b) =>
        {
            int tierCompare = a.Personality.tier.CompareTo(b.Personality.tier);
            if (tierCompare != 0)
                return tierCompare;

            return a.Specialization.CompareTo(b.Specialization);
        });

        // 정렬 이후 인덱스 재정렬
        ReindexAllSpecializations();
    }

    /// <summary>
    /// 모든 특화 인덱스를 전부 재계산합니다.
    /// </summary>
    private void ReindexAllSpecializations()
    {
        foreach (SpecializationType type in System.Enum.GetValues(typeof(SpecializationType)))
        {
            ReindexSpecialization(type);
        }
    }
}
