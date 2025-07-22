using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    [SerializeField] private AssistantInfoView infoViewPrefab; // 프리팹 (UI에 제자 정보 표시)
    [SerializeField] private Transform infoContainer; // 프리팹을 붙일 위치

    private AssistantFactory assistantFactory;
    private List<AssistantInstance> currentCandidates = new();

    public void Init(AssistantFactory factory)
    {
        assistantFactory = factory;
    }

    /// <summary>
    /// 1회 영입 버튼에서 호출
    /// </summary>
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

    private void CreateInfoView(AssistantInstance instance)
    {
        var view = Instantiate(infoViewPrefab, infoContainer);
        view.SetData(instance);
    }

    private void ClearCurrentDisplay()
    {
        foreach (Transform child in infoContainer)
        {
            Destroy(child.gameObject);
        }
        currentCandidates.Clear();
    }
}
