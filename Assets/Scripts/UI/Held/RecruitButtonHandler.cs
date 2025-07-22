using UnityEngine;
using UnityEngine.UI;

public class RecruitButtonHandler : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button btnRecruitOne;
    [SerializeField] private Button btnApprove;
    [SerializeField] private Button btnReject;
    [SerializeField] private Button btnHold;

    [Header("참조 스크립트")]
    [SerializeField] private RecruitPreviewManager previewManager;

    private void Awake()
    {
        btnRecruitOne.onClick.AddListener(OnClickRecruitOne);
        btnApprove.onClick.AddListener(OnClickApprove);
        btnReject.onClick.AddListener(OnClickReject);
        btnHold.onClick.AddListener(OnClickHold);
    }

    // 제자 1명 뽑기
    public void OnClickRecruitOne()
    {
        previewManager.TryRecruitCandidate();
    }

    // 영입 확정
    public void OnClickApprove()
    {
        previewManager.ApproveCandidate();
    }

    // 영입 거절
    public void OnClickReject()
    {
        previewManager.RejectCandidate();
    }

    // 보류 처리
    public void OnClickHold()
    {
        previewManager.HoldCandidate();
    }
}
