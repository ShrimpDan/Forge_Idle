using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자 데이터의 리스트를 관리하고, 추가/삭제/조회/정렬 기능을 담당합니다.
/// </summary>
public class TraineeInventory
{
    private readonly List<TraineeData> traineeList = new();

    /// <summary>
    /// 제자를 리스트에 추가하고 특화 인덱스를 갱신합니다.
    /// </summary>
    public void Add(TraineeData data)
    {
        traineeList.Add(data);
        ReindexSpecialization(data.Specialization);
    }

    /// <summary>
    /// 제자를 리스트에서 제거하고 특화 인덱스를 다시 계산합니다.
    /// </summary>
    public void Remove(TraineeData data)
    {
        traineeList.Remove(data);
        ReindexSpecialization(data.Specialization);
    }

    /// <summary>
    /// 전체 제자 리스트를 반환합니다.
    /// </summary>
    public List<TraineeData> GetAll()
    {
        return traineeList;
    }

    /// <summary>
    /// 특정 특화 타입의 제자들만 반환합니다.
    /// </summary>
    public List<TraineeData> GetBySpecialization(SpecializationType type)
    {
        return traineeList.FindAll(t => t.Specialization == type);
    }

    /// <summary>
    /// 현재 저장된 제자들을 디버그 로그로 출력합니다.
    /// </summary>
    public void DebugPrint()
    {
        Debug.Log($"[전체 제자 수]: {traineeList.Count}");
        for (int i = 0; i < traineeList.Count; i++)
        {
            var t = traineeList[i];
            Debug.Log($"[{i + 1}] 이름: {t.Name} / 특화: {t.Specialization} / 번호: {t.SpecializationIndex}");
        }
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
}
