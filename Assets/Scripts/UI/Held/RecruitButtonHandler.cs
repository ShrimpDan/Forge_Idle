using UnityEngine;
using UnityEngine.UI;

public class RecruitButtonHandler : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button btnRecruitRandom;
    [SerializeField] private Button btnRecruitCrafting;
    [SerializeField] private Button btnRecruitSelling;
    [SerializeField] private Button btnRecruitMining;
    [SerializeField] private Button btnApprove;
    [SerializeField] private Button btnReject;
    [SerializeField] private Button btnHold;

    [Header("참조 스크립트")]
    [SerializeField] private RecruitPreviewManager previewManager;

    private void Awake()
    {
        btnRecruitRandom.onClick.AddListener(() => OnClickRecruitByType(null));
        btnRecruitCrafting.onClick.AddListener(() => OnClickRecruitByType(SpecializationType.Crafting));
        btnRecruitSelling.onClick.AddListener(() => OnClickRecruitByType(SpecializationType.Selling));
        btnRecruitMining.onClick.AddListener(() => OnClickRecruitByType(SpecializationType.Mining));

        btnApprove.onClick.AddListener(OnClickApprove);
        btnReject.onClick.AddListener(OnClickReject);
        btnHold.onClick.AddListener(OnClickHold);
    }

    // 선택한 특화에 따라 제자 뽑기
    private void OnClickRecruitByType(SpecializationType? type)
    {
        previewManager.TryRecruitCandidateByType(type);
    }

    // 영입 확정
    public void OnClickApprove() => previewManager.ApproveCandidate();

    // 영입 거절
    public void OnClickReject() => previewManager.RejectCandidate();

    // 보류 처리
    public void OnClickHold() => previewManager.HoldCandidate();
}
