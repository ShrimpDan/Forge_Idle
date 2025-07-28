using UnityEngine;
using UnityEngine.UI;

// RecruitButtonHandler.cs
// 제자 영입 UI에서 버튼 클릭 이벤트를 처리합니다.
// 랜덤 및 특화 영입 버튼과 영입 확정/거절/보류 버튼의 클릭 이벤트를 담당합니다.

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

    // 버튼 리스너 설정
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
    public void OnClickApprove()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        previewManager.ApproveCandidate();
    }

    // 영입 거절
    public void OnClickReject()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        previewManager.RejectCandidate();
    }

    // 보류 처리
    public void OnClickHold()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        previewManager.HoldCandidate();
    }
}
