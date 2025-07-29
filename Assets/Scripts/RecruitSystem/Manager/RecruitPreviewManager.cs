using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// RecruitPreviewManager.cs
// 제자 영입 프리뷰를 관리하는 UI 컨트롤러입니다.
// 제자 5명을 생성하고 UI에 순차적으로 표시하며, 영입/거절/보류 처리를 담당합니다.
// 보류 중인 제자를 다시 불러와 확인하는 기능도 포함합니다.

public class RecruitPreviewManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RecruitPopup popup;
    [SerializeField] private RecruitConfirmPopup confirmPopup;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Transform paperRoot;
    [SerializeField] private GameObject recruitUI;
    [SerializeField] private GameObject heldUI;

    [Header("Button")]
    [SerializeField] private Button btnApprove;
    [SerializeField] private Button btnReject;
    [SerializeField] private Button btnHold;

    private AssistantFactory assistantFactory;
    private SpecializationType? recruitFilter = null;

    private List<AssistantInstance> candidatePool = new();
    private List<GameObject> activePapers = new();
    private GameObject currentPaperGO;

    private bool isFromHeldList = false;
    private AssistantInstance currentHeldInstance = null;
    private bool isTransitioning = false;
    private int currentIndex = 0;

    // 팩토리 초기화
    private void Start()
    {
        if (GameManager.Instance?.DataManager == null)
        {
            Debug.LogError("[RecruitPreviewManager] GameManager 또는 DataManager 가 null");
            return;
        }

        assistantFactory = new AssistantFactory(GameManager.Instance.DataManager);
    }

    // 랜덤/특화 제자 뽑기 시도 (보류 제자 존재 여부 확인 포함)
    public void TryRecruitCandidateByType(SpecializationType? type)
    {
        recruitFilter = type;

        if (GameManager.Instance.HeldCandidates.Count > 0)
        {
            confirmPopup.Show(
                "보류 중인 제자가 있습니다.\n삭제 후 새로 뽑기를 진행하시겠습니까?",
                onConfirm: () =>
                {
                    GameManager.Instance.HeldCandidates.Clear();
                    GameManager.Instance.SaveManager.SaveAll();
                    StartRecruit();
                },
                onCancel: () => { });
        }
        else
        {
            StartRecruit();
        }
    }

    // 실제로 제자 5명 생성 후 프리뷰 시작
    private void StartRecruit()
    {
        ClearActivePapers();
        currentIndex = 0;
        currentHeldInstance = null;
        isFromHeldList = false;
        currentPaperGO = null;
        isTransitioning = false;

        recruitUI?.SetActive(true);
        SetButtonsInteractable(false);

        var allExistingKeys = new HashSet<string>(
            GameManager.Instance.AssistantInventory.GetAll().Select(a => a.Key)
            .Concat(GameManager.Instance.HeldCandidates.Select(a => a.Key)));

        candidatePool.Clear();
        int attempts = 0;
        const int maxAttempts = 30;

        while (candidatePool.Count < 5 && attempts < maxAttempts)
        {
            var candidate = recruitFilter == null
                ? assistantFactory.CreateRandomTrainee(true)
                : assistantFactory.CreateFixedTrainee(recruitFilter.Value, true);

            if (candidate != null && !allExistingKeys.Contains(candidate.Key) &&
                !candidatePool.Any(c => c.Key == candidate.Key))
            {
                candidatePool.Add(candidate);
            }

            attempts++;
        }

        if (candidatePool.Count == 0)
        {
            Debug.LogWarning("[Recruit] 유효한 후보가 없습니다.");
            assistantFactory.ResetRecruitLock();
            popup.HidePopup();
            return;
        }

        ShowCurrentCandidate();
    }

    // 현재 제자 후보를 화면에 표시
    private void ShowCurrentCandidate()
    {
        if (currentIndex < 0 || currentIndex >= candidatePool.Count)
        {
            Debug.LogWarning($"[Recruit] 잘못된 인덱스 접근 시도: currentIndex={currentIndex}, 총 개수={candidatePool.Count}");
            EndRecruitFlow();
            return;
        }

        var candidate = candidatePool[currentIndex];
        isTransitioning = true;
        SetButtonsInteractable(false);

        void OnPaperReady()
        {
            isTransitioning = false;
            SetButtonsInteractable(true);
        }

        if (currentPaperGO != null)
        {
            var oldPaper = currentPaperGO.GetComponent<AssistantPaperAnimator>();
            oldPaper?.AnimateExitToTopRight(onComplete: () =>
            {
                activePapers.Remove(currentPaperGO);
                currentPaperGO = null;

                currentPaperGO = CreatePaper(candidate, OnPaperReady);
            });
        }
        else
        {
            currentPaperGO = CreatePaper(candidate, OnPaperReady);
        }

        popup.ShowPopup(candidate);
    }

    // 제자 종이 생성 및 애니메이션 재생
    private GameObject CreatePaper(AssistantInstance data, Action onEnterComplete = null)
    {
        var paper = Instantiate(paperPrefab, paperRoot);
        var animator = paper.GetComponent<AssistantPaperAnimator>();
        var infoView = paper.GetComponent<AssistantInfoView>();

        infoView.SetData(data);
        SoundManager.Instance.Play("SFX_RecruitPaperFlip");
        animator?.AnimateEnterFromTopLeft(onComplete: onEnterComplete);

        activePapers.Add(paper);
        return paper;
    }

    // 모든 종이 제거
    private void ClearActivePapers()
    {
        foreach (var paper in activePapers)
        {
            if (paper != null)
                Destroy(paper);
        }

        activePapers.Clear();
        currentPaperGO = null;
    }

    // 제자 영입 확정
    public void ApproveCandidate()
    {
        if (isTransitioning) return;
        SetButtonsInteractable(false);
        var gm = GameManager.Instance;

        if (!isFromHeldList && (currentIndex < 0 || currentIndex >= candidatePool.Count))
        {
            Debug.LogWarning($"[Recruit] ApproveCandidate에서 잘못된 인덱스 접근: {currentIndex}");
            EndRecruitFlow();
            return;
        }

        var target = isFromHeldList ? currentHeldInstance : candidatePool[currentIndex];
        int cost = target.RecruitCost;

        if (!gm.ForgeManager.UseGold(cost))
        {
            Debug.LogWarning("[Recruit] 골드 부족");
            SetButtonsInteractable(true);
            return;
        }

        gm.AssistantInventory.Add(target);

        if (isFromHeldList)
            gm.HeldCandidates.Remove(currentHeldInstance);

        gm.SaveManager.SaveAll();

        if (isFromHeldList)
        {
            ReturnToProperUI();
            return;
        }

        AdvanceToNextCandidate();
    }


    // 제자 영입 거절
    public void RejectCandidate()
    {
        if (isTransitioning) return;
        SetButtonsInteractable(false);

        if (isFromHeldList)
        {
            GameManager.Instance.HeldCandidates.Remove(currentHeldInstance);
            GameManager.Instance.SaveManager.SaveAll();
            ReturnToProperUI();
            return;
        }

        AdvanceToNextCandidate();
    }


    // 제자 보류 처리
    public void HoldCandidate()
    {
        if (isTransitioning) return;
        SetButtonsInteractable(false);

        if (isFromHeldList)
        {
            ReturnToProperUI();
            return;
        }

        if (currentIndex < 0 || currentIndex >= candidatePool.Count)
        {
            Debug.LogWarning($"[Recruit] HoldCandidate에서 잘못된 인덱스 접근: {currentIndex}");
            EndRecruitFlow();
            return;
        }

        var held = candidatePool[currentIndex];
        GameManager.Instance.HeldCandidates.Add(held);
        GameManager.Instance.SaveManager.SaveAll();

        AdvanceToNextCandidate();
    }

    private void AdvanceToNextCandidate()
    {
        currentIndex++;

        if (currentIndex >= candidatePool.Count)
        {
            EndRecruitFlow();
        }
        else
        {
            ShowCurrentCandidate();
        }
    }

    private void EndRecruitFlow()
    {
        popup.HidePopup();
        ClearActivePapers();
        currentPaperGO = null;
        SetButtonsInteractable(false);
        recruitUI.SetActive(false);
    }


    // 보류 제자 프리뷰 호출
    public void ShowSingleCandidateFromHeld(AssistantInstance held)
    {
        isFromHeldList = true;
        currentHeldInstance = held;

        heldUI.SetActive(false);
        recruitUI.SetActive(true);

        popup.ShowPopup(held);

        if (currentPaperGO != null)
            Destroy(currentPaperGO);

        currentPaperGO = CreatePaper(held, () =>
        {
            isTransitioning = false;
            SetButtonsInteractable(true);
        });

        SetButtonsInteractable(false);
    }

    // UI 상태 복귀
    private void ReturnToProperUI()
    {
        popup.HidePopup();

        if (isFromHeldList)
        {
            recruitUI.SetActive(false);
            heldUI.SetActive(true);
            SetButtonsInteractable(false);

            GameManager.Instance.UIManager.HeldAssistantUIController
                .ShowHeldAssistantList(GameManager.Instance.HeldCandidates);
        }

        isFromHeldList = false;
        currentHeldInstance = null;
    }

    // 버튼 활성화 상태 일괄 설정
    private void SetButtonsInteractable(bool interactable)
    {
        btnApprove.interactable = interactable;
        btnReject.interactable = interactable;
        btnHold.interactable = interactable;
    }

    // 버튼 이벤트 (랜덤/특화별)
    public void OnClickRecruitRandom() => TryRecruitCandidateByType(SpecializationType.All);
    public void OnClickRecruitCrafting() => TryRecruitCandidateByType(SpecializationType.Crafting);
    public void OnClickRecruitMining() => TryRecruitCandidateByType(SpecializationType.Mining);
    public void OnClickRecruitSelling() => TryRecruitCandidateByType(SpecializationType.Selling);
}
