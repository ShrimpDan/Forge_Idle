using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitPreviewManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RecruitPopup popup;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Transform paperRoot;
    [SerializeField] private GameObject recruitUI;

    [Header("Button")]
    [SerializeField] private Button btnApprove;
    [SerializeField] private Button btnReject;
    [SerializeField] private Button btnHold;

    private AssistantFactory assistantFactory;
    private List<AssistantInstance> candidatePool = new();
    private List<AssistantInstance> heldCandidates = new();
    private List<GameObject> activePapers = new();
    private GameObject currentPaperGO;

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

    public void TryRecruitCandidate()
    {
        foreach (var paper in activePapers)
        {
            if (paper != null)
                Destroy(paper);
        }
        activePapers.Clear();

        currentIndex = 0;
        heldCandidates.Clear();
        currentPaperGO = null;

        recruitUI?.SetActive(true);

        candidatePool = assistantFactory.CreateMultiple(5);

        if (candidatePool.Count == 0)
        {
            Debug.LogWarning("[Recruit] 뽑힌 후보가 없습니다.");
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

            foreach (var paper in activePapers)
            {
                if (paper != null)
                    Destroy(paper);
            }

            activePapers.Clear();
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
                currentPaperGO = CreatePaper(candidate, OnPaperReady);
            });
        }
        else
        {
            currentPaperGO = CreatePaper(candidate, OnPaperReady);
        }
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

    public void ApproveCandidate()
    {
        if (isTransitioning) return;

        var approved = candidatePool[currentIndex];
        GameManager.Instance.AssistantInventory.Add(approved);

        currentIndex++;
        ShowCurrentCandidate();
    }

    public void RejectCandidate()
    {
        if (isTransitioning) return;

        currentIndex++;
        ShowCurrentCandidate();
    }

    public void HoldCandidate()
    {
        if (isTransitioning) return;

        heldCandidates.Add(candidatePool[currentIndex]);
        currentIndex++;
        ShowCurrentCandidate();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        btnApprove.interactable = interactable;
        btnReject.interactable = interactable;
        btnHold.interactable = interactable;
    }
}
