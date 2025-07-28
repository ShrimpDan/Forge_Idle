using System.Collections.Generic;
using UnityEngine;

// RecruitManager.cs
// AssistantFactory를 통해 생성한 제자 데이터를 UI에 표시합니다.
// 1회 영입 기능을 지원하며, 생성된 제자 정보를 프리팹으로 인스턴스화해 보여줍니다.

public class RecruitManager : MonoBehaviour
{
    [SerializeField] private AssistantInfoView infoViewPrefab;  // 프리팹 (UI에 제자 정보 표시)
    [SerializeField] private Transform infoContainer;           // 프리팹을 붙일 위치

    private AssistantFactory assistantFactory;
    private List<AssistantInstance> currentCandidates = new();

    // 팩토리 초기화
    public void Init(AssistantFactory factory)
    {
        assistantFactory = factory;
    }

    // 제자 1명 영입 요청
    public void RecruitOne()
    {
        ClearCurrentDisplay();

        var assistant = assistantFactory.CreateRandomTrainee();
        if (assistant == null)
        {
            Debug.LogWarning("영입 가능한 제자가 없습니다.");
            return;
        }

        currentCandidates.Add(assistant);
        CreateInfoView(assistant);
    }

    // 제자 UI 생성
    private void CreateInfoView(AssistantInstance instance)
    {
        var view = Instantiate(infoViewPrefab, infoContainer);
        view.SetData(instance);
    }

    // 기존 제자 UI 제거
    private void ClearCurrentDisplay()
    {
        foreach (Transform child in infoContainer)
        {
            Destroy(child.gameObject);
        }
        currentCandidates.Clear();
    }
}
