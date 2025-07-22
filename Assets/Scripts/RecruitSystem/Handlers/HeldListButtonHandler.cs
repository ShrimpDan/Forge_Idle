using System.Collections.Generic;
using UnityEngine;

public class HeldListButtonHandler : MonoBehaviour
{
    [SerializeField] private HeldAssistantUIController heldUIController;
    [SerializeField] private GameObject recruitUI;
    [SerializeField] private GameObject heldUI;
    [SerializeField] private RecruitPreviewManager previewManager;

    /// <summary>
    /// 보류 제자 리스트 열기
    /// </summary>
    public void OnClickOpenHeldList()
    {
        List<AssistantInstance> heldList = GameManager.Instance?.HeldCandidates;
        if (heldList == null || heldList.Count == 0)
        {
            Debug.Log("[보류 제자] 보류 중인 제자가 없습니다.");
            return;
        }

        heldUIController.ShowHeldAssistantList(heldList);
    }

    /// <summary>
    /// 보류 제자 중 하나를 클릭했을 때 호출
    /// </summary>
    public void OnClickHeldAssistant(AssistantInstance assistant)
    {
        heldUI.SetActive(false);
        recruitUI.SetActive(true);

        previewManager.ShowSingleCandidateFromHeld(assistant);
    }
}
