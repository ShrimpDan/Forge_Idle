using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        inventory = new AssistantInventory(gameManager.ForgeManager);
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

    public void DismissAssistant(AssistantInstance target)
    {
        if (target == null) return;

        if (target.IsEquipped)
        {
            gameManager.ForgeManager.EquippedAssistant[target.EquippedForge][target.Specialization] = null;
            target.EquipAssi(false);
        }

        if (target.IsInUse)
        {
            target.IsInUse = false;
        }

        GameManager.Instance.HeldCandidates.RemoveAll(c => c.Key == target.Key);

        AssistantInventory.Remove(target);

        GameManager.Instance.SaveManager.SaveAll();
    }


    public void RecruitSingle(SpecializationType? type = null)
    {
        if (!canRecruit)
        {
            Debug.LogWarning("[AssistantManager] RecruitSingle 호출이 차단됨 (이미 진행 중)");
            return;
        }

        currentBatch.Clear();

        AssistantController.ResetSoundOnce();

        HandleSingleRecruit(() => {
            var ownedKeys = inventory.GetAll().Select(a => a.Key).ToHashSet();
            return factory.CreateSmartRandomTrainee(ownedKeys, type);
        });
    }

    public void RecruitMultiple(int count, SpecializationType? type = null)
    {
        if (!canRecruit) return;
        canRecruit = false;
        drawController.StartMultipleRecruit(count, type);
    }

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

    public void ConfirmTrainee(AssistantInstance data)
    {
        currentBatch.Add(data);
        inventory.Add(data);
    }

    public void RemoveTrainee(GameObject obj, AssistantInstance data)
    {
        inventory.Remove(data);
        Destroy(obj);
        canRecruit = true;

        if (backgroundPanel != null)
            backgroundPanel.SetActive(false);
    }

    public void DebugPrintAllAssistant() => inventory.DebugPrint();
    public List<AssistantInstance> GetAllAssistant() => inventory.GetAll();
    public List<AssistantInstance> GetAssistantByType(SpecializationType type) => inventory.GetBySpecialization(type);
    public List<AssistantInstance> GetLastDrawResult() => currentBatch;
}
