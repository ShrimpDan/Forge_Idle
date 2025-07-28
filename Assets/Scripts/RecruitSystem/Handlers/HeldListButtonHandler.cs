using System.Collections.Generic;
using UnityEngine;

// HeldListButtonHandler.cs
// 보류 제자 UI를 열고, 제자를 선택했을 때 영입 화면으로 전환하는 역할을 담당합니다.
// UI 전환 및 선택한 제자의 프리뷰 연동을 처리합니다.

public class HeldListButtonHandler : MonoBehaviour
{
    [SerializeField] private HeldAssistantUIController heldUIController;
    [SerializeField] private GameObject recruitUI;
    [SerializeField] private GameObject heldUI;
    [SerializeField] private RecruitPreviewManager previewManager;

    // 보류 제자 리스트 열기
    public void OnClickOpenHeldList()
    {
        List<AssistantInstance> heldList = GameManager.Instance?.HeldCandidates;

        if (heldList == null || heldList.Count == 0)
        {
            Debug.Log("[보류 제자] 보류 중인 제자가 없습니다.");
            return;
        }

        SoundManager.Instance.Play("SFX_SystemClick");
        heldUIController.ShowHeldAssistantList(heldList);
    }

    // 보류 제자 중 하나를 클릭했을 때 호출됨 -> 영입 UI로 전환
    public void OnClickHeldAssistant(AssistantInstance assistant)
    {
        heldUI.SetActive(false);
        recruitUI.SetActive(true);

        previewManager.ShowSingleCandidateFromHeld(assistant);
    }
}
