using System.Collections.Generic;
using UnityEngine;

public class RecruitPreviewManager : MonoBehaviour
{
    [SerializeField] private RecruitPopup popup;

    private AssistantFactory assistantFactory;
    private List<AssistantInstance> candidatePool = new();
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

    /// <summary>
    /// 5명의 후보를 뽑고 첫 번째 제자 보여주기
    /// </summary>
    public void TryRecruitCandidate()
    {
        candidatePool = assistantFactory.CreateMultiple(5);
        currentIndex = 0;

        if (candidatePool.Count == 0)
        {
            Debug.LogWarning("[Recruit] 뽑힌 후보가 없습니다.");
            popup.HidePopup();
            return;
        }

        ShowCurrentCandidate();
    }

    /// <summary>
    /// 현재 제자 UI에 출력
    /// </summary>
    private void ShowCurrentCandidate()
    {
        if (currentIndex >= candidatePool.Count)
        {
            popup.HidePopup();
            return;
        }

        var currentCandidate = candidatePool[currentIndex];
        popup.ShowPopup(currentCandidate);
    }

    public void ApproveCandidate()
    {
        var approved = candidatePool[currentIndex];
        GameManager.Instance.AssistantInventory.Add(approved);

        currentIndex++;
        ShowCurrentCandidate();
    }

    public void RejectCandidate()
    {
        currentIndex++;
        ShowCurrentCandidate();
    }

    public void HoldCandidate()
    {
        // 보류 처리 시, 인벤토리에 넣지 않지만 현재 로직에선 딱히 저장 구조 없음
        // 나중에 보류 리스트에 따로 저장해도 됨
        currentIndex++;
        ShowCurrentCandidate();
    }
}
