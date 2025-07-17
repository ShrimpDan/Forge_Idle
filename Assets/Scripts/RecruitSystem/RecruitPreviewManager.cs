using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RecruitPreviewManager : MonoBehaviour
{
    [SerializeField] private RecruitPopup popup;

    private AssistantFactory assistantFactory;
    private AssistantInstance currentCandidate;
    private List<AssistantInstance> candidatePool = new();
    private AssistantInstance heldCandidate = null;

    private void Start()
    {
        if (GameManager.Instance?.DataManager == null)
        {
            Debug.LogError("[RecruitPreviewManager] GameManager 또는 DataManager 가 null");
            return;
        }

        assistantFactory = new AssistantFactory(GameManager.Instance.DataManager);
        InitializeCandidatePool();
    }

    private void InitializeCandidatePool()
    {
        candidatePool = assistantFactory.CreateMultiple(10);
    }

    public void TryRecruitCandidate()
    {
        if (heldCandidate != null)
        {
            currentCandidate = heldCandidate;
            Debug.Log($"[Recruit] 보류된 제자 다시 표시: {currentCandidate.Name}");
            popup.ShowPopup(currentCandidate);
            return;
        }

        // 새로 뽑기
        if (candidatePool.Count == 0)
        {
            Debug.Log("[Recruit] 제자 리스트가 비어있습니다.");
            popup.HidePopup();
            return;
        }

        currentCandidate = candidatePool[Random.Range(0, candidatePool.Count)];
        popup.ShowPopup(currentCandidate);
    }

    public void ApproveCandidate()
    {
        candidatePool.Remove(currentCandidate);
        heldCandidate = null;
        currentCandidate = null;
        popup.HidePopup();
    }

    public void RejectCandidate()
    {
        heldCandidate = null;
        currentCandidate = null;
        popup.HidePopup();
    }

    public void HoldCandidate()
    {
        if (currentCandidate != null)
        {
            heldCandidate = currentCandidate;
            Debug.Log($"[Recruit] 보류: {heldCandidate.Name}");
        }
        popup.HidePopup();
    }
}
