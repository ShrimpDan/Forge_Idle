using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    private SpecializationType? recruitFilter = null;

    private AssistantFactory assistantFactory;
    private List<AssistantInstance> candidatePool = new();
    private List<GameObject> activePapers = new();
    private GameObject currentPaperGO;

    private bool isFromHeldList = false;
    private AssistantInstance currentHeldInstance = null;
    private bool isTransitioning = false;
    private int currentIndex = 0;

    private void Start()
    {
        if (GameManager.Instance?.DataManager == null)
        {
            Debug.LogError("[RecruitPreviewManager] GameManager 또는 DataManager 가 null");
            return;
        }

        assistantFactory = new AssistantFactory(GameManager.Instance.DataManager);
    }

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

        // 중복 방지를 위한 키셋 구성
        var inventoryKeys = GameManager.Instance.AssistantInventory.GetAll().Select(a => a.Key);
        var heldKeys = GameManager.Instance.HeldCandidates.Select(a => a.Key);
        var allExistingKeys = new HashSet<string>(inventoryKeys.Concat(heldKeys));

        // 중복 제거하며 최대 5명 뽑기
        candidatePool.Clear();
        int attempts = 0;
        int maxAttempts = 30;

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
            popup.HidePopup();
            return;
        }

        ShowCurrentCandidate();
    }

    private void ShowCurrentCandidate()
    {
        if (currentIndex >= candidatePool.Count)
        {
            popup.HidePopup();
            ClearActivePapers();
            currentPaperGO = null;
            isTransitioning = false;
            SetButtonsInteractable(false);
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

    private GameObject CreatePaper(AssistantInstance data, Action onEnterComplete = null)
    {
        var paper = Instantiate(paperPrefab, paperRoot);
        var animator = paper.GetComponent<AssistantPaperAnimator>();
        var infoView = paper.GetComponent<AssistantInfoView>();

        infoView.SetData(data);
        animator?.AnimateEnterFromTopLeft(onComplete: onEnterComplete);

        activePapers.Add(paper);
        return paper;
    }

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

    public void ApproveCandidate()
    {
        if (isTransitioning) return;

        SetButtonsInteractable(false);
        var gm = GameManager.Instance;

        if (isFromHeldList)
        {
            int cost = currentHeldInstance.RecruitCost;
            if (!gm.ForgeManager.UseGold(cost))
            {
                Debug.LogWarning("[Recruit] 골드 부족");
                SetButtonsInteractable(true);
                return;
            }

            gm.AssistantInventory.Add(currentHeldInstance);
            gm.HeldCandidates.Remove(currentHeldInstance);
            gm.SaveManager.SaveAll();

            ReturnToProperUI();
            return;
        }

        var approved = candidatePool[currentIndex];
        int recruitCost = approved.RecruitCost;

        if (!gm.ForgeManager.UseGold(recruitCost))
        {
            Debug.LogWarning("[Recruit] 골드 부족");
            SetButtonsInteractable(true);
            return;
        }

        gm.AssistantInventory.Add(approved);
        gm.SaveManager.SaveAll();

        currentIndex++;
        ShowCurrentCandidate();
    }

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

        currentIndex++;
        ShowCurrentCandidate();
    }

    public void HoldCandidate()
    {
        if (isTransitioning) return;

        SetButtonsInteractable(false);

        if (isFromHeldList)
        {
            ReturnToProperUI();
            return;
        }

        var held = candidatePool[currentIndex];
        GameManager.Instance.HeldCandidates.Add(held);
        GameManager.Instance.SaveManager.SaveAll();

        currentIndex++;
        ShowCurrentCandidate();
    }

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

    private void SetButtonsInteractable(bool interactable)
    {
        btnApprove.interactable = interactable;
        btnReject.interactable = interactable;
        btnHold.interactable = interactable;
    }
}
