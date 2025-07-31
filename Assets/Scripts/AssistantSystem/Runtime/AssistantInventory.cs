using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자 데이터의 리스트를 관리하고, 추가/삭제/조회/정렬 기능을 담당합니다.
/// </summary>
public class AssistantInventory
{
    private ForgeManager forgeManager;
    private List<AssistantInstance> assistantList = new();

    public AssistantInventory(ForgeManager forgeManager)
    {
        this.forgeManager = forgeManager;
    }

    /// <summary>
    /// 제자를 리스트에 추가하고 특화 인덱스를 갱신합니다.
    /// </summary>
    public void Add(AssistantInstance data)
    {
        if (data == null)
        {
            Debug.LogWarning("[AssistantInventory] Null 데이터를 추가하려고 시도했습니다.");
            return;
        }

        assistantList.Add(data);
        ReindexSpecialization(data.Specialization);
    }

    /// <summary>
    /// 제자를 리스트에서 제거하고 특화 인덱스를 다시 계산합니다.
    /// </summary>
    public void Remove(AssistantInstance data)
    {
        if (!assistantList.Remove(data))
        {
            Debug.LogWarning("[AssistantInventory] 리스트에서 제거 실패: 해당 제자가 존재하지 않습니다.");
            return;
        }

        ReindexSpecialization(data.Specialization);
    }

    /// <summary>
    /// 보유중인 모든 제자를 삭제합니다.
    /// </summary>
    public void Clear()
    {
        assistantList.Clear();
        Debug.Log("[AssistantInventory] 제자 인벤토리 초기화 완료!");
    }

    /// <summary>
    /// 전체 제자 리스트를 반환합니다.
    /// </summary>
    public List<AssistantInstance> GetAll()
    {
        return new List<AssistantInstance>(assistantList); // 보호용 복사본 반환
    }

    /// <summary>
    /// 특정 특화 타입의 제자들만 반환합니다.
    /// </summary>
    public List<AssistantInstance> GetBySpecialization(SpecializationType type)
    {
        return assistantList.FindAll(t => t.Specialization == type);
    }

    /// <summary>
    /// 장착 중인 제자들을 반환합니다.
    /// </summary>
    public List<AssistantInstance> GetEquippedTrainees()
    {
        return assistantList.FindAll(t => t.IsEquipped);
    }

    /// <summary>
    /// 디버그 용도로 제자 리스트 전체를 출력합니다.
    /// </summary>
    public void DebugPrint()
    {
        Debug.Log($"[전체 제자 수]: {assistantList.Count}");
        for (int i = 0; i < assistantList.Count; i++)
        {
            var t = assistantList[i];
            Debug.Log($"[{i + 1}] {ToStringAssistant(t)}");
        }
    }

    private string ToStringAssistant(AssistantInstance data)
    {
        return $"이름: {data.Name} / 특화: {data.Specialization} / 인덱스: {data.SpecializationIndex} / 레벨: {data.Level}";
    }

    /// <summary>
    /// 해당 특화 타입의 제자 인덱스를 재정렬합니다.
    /// </summary>
    private void ReindexSpecialization(SpecializationType type)
    {
        var sameType = assistantList.FindAll(t => t.Specialization == type);
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
        assistantList.Sort((a, b) =>
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

    /// <summary>
    /// 사용 가능한 제자만 반환
    /// </summary>
    public List<AssistantInstance> GetActiveTrainees()
    {
        return assistantList.FindAll(t => !t.IsFired);
    }

    /// <summary>
    /// 시급을 지불하지 못해 사용 못하는 제자만 반환
    /// </summary>
    public List<AssistantInstance> GetFiredTrainees()
    {
        return assistantList.FindAll(t => t.IsFired);
    }

    public AssistantInstance GetAssistantInstance(string key)
    {
        return assistantList.Find(a => a.Key == key);
    }

    // 마인씬용 사용 가능 목록
    public List<AssistantInstance> GetAvailableForMine()
    {
        return assistantList.FindAll(a => !a.IsFired && !a.IsEquipped && !a.IsInUse);
    }
}