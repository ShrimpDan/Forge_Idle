using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelTileEngine.Scripts;
using UnityEngine;

public class AssistantManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("카드 프리팹")]
    [SerializeField] private GameObject largeTraineeCardPrefab;
    [SerializeField] private GameObject miniTraineeCardPrefab;

    [Header("버튼 프리팹")]
    [SerializeField] private GameObject confirmAllButtonPrefab;

    [Header("카드 출력 위치")]
    [SerializeField] private Transform singleDrawParent;
    [SerializeField] private Transform multiDrawParent;

    [Header("애니메이터")]
    [SerializeField] private AssistantCardLayoutAnimator layoutAnimator;

    [Header("Draw 컨트롤러")]
    [SerializeField] private AssistantDrawController drawController;

    [Header("UI 배경")]
    [SerializeField] private GameObject backgroundPanel;

    [SerializeField] private FusionUIController fusionUIController;

    private AssistantFactory factory;
    private AssistantCardSpawner spawner;
    private AssistantInventory inventory;
    public AssistantInventory AssistantInventory => inventory;
    public AssistantCardSpawner Spawner => spawner;

    private List<AssistantInstance> currentBatch = new();
    private bool canRecruit = true;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        
        inventory = new AssistantInventory(gameManager.Forge);
        factory = new AssistantFactory(gameManager.DataManager);
        spawner = new AssistantCardSpawner(
            largeTraineeCardPrefab,
            miniTraineeCardPrefab,
            singleDrawParent,
            multiDrawParent,
            factory,
            drawController
        );

        drawController.Init(factory, spawner);
        drawController.OnTraineeConfirmed += ConfirmTrainee;
        drawController.OnRecruitingFinished += () => {
            canRecruit = true;

            if (backgroundPanel != null)
                backgroundPanel.SetActive(false);
        };
    }

    [ContextMenu("제자 인벤토리 초기화")]
    public void ClearAllAssistants()
    {
        inventory.Clear();
        currentBatch.Clear();

        if (fusionUIController != null)
            fusionUIController.SetFilteredMode(false);

        var saveHandler = new AssistantSaveHandler(this, GameManager.Instance.DataManager.PersonalityLoader);
        saveHandler.Delete();

        Debug.Log("<color=red>[AssistantManager] 제자 전체 초기화 및 저장 삭제 완료</color>");
    }

    // 단일 뽑기 처리 (외부에서 호출)
    public void RecruitSingle(SpecializationType? type = null)
    {
        AssistantController.ResetSoundOnce();

        HandleSingleRecruit(() =>
            type == null
                ? factory.CreateRandomTrainee()
                : factory.CreateFixedTrainee(type.Value)
        );
    }

    // 다중 뽑기 처리
    public void RecruitMultiple(int count, SpecializationType? type = null)
    {
        if (!canRecruit) return;
        canRecruit = false;
        drawController.StartMultipleRecruit(count, type);
    }

    // 내부 처리 함수
    private void HandleSingleRecruit(System.Func<AssistantInstance> recruitFunc)
    {
        if (!canRecruit) return;
        canRecruit = false;

        StartCoroutine(SingleRecruitFlow(recruitFunc));
    }

    private IEnumerator SingleRecruitFlow(System.Func<AssistantInstance> recruitFunc)
    {
        if (backgroundPanel != null)
            backgroundPanel.SetActive(true);

        yield return null;

        var data = recruitFunc.Invoke();
        if (data == null) yield break;

        var obj = spawner.SpawnLargeCard(data);
        ConfirmTrainee(data);
    }

    // 외부에서 제자 등록
    public void ConfirmTrainee(AssistantInstance data)
    {
        currentBatch.Add(data);
        inventory.Add(data);
    }

    // 외부에서 제자 제거
    public void RemoveTrainee(GameObject obj, AssistantInstance data)
    {
        inventory.Remove(data);
        Destroy(obj);
        canRecruit = true;

        if (backgroundPanel != null)
            backgroundPanel.SetActive(false);
    }

    // 디버그 / 데이터 접근
    public void DebugPrintAllAssistant() => inventory.DebugPrint();
    public List<AssistantInstance> GetAllAssistant() => inventory.GetAll();
    public List<AssistantInstance> GetAssistantByType(SpecializationType type) => inventory.GetBySpecialization(type);
    public List<AssistantInstance> GetLastDrawResult() => currentBatch;
}
