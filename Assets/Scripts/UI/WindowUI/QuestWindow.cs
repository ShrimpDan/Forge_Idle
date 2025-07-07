using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform questSlotRoot;
    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private ScrollRect scrollRect;

    private List<QuestSlot> questSlots = new();

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.QuestWindow));
    }

    public void OpenQuests()
    {
        var questList = gameManager.DataManager.QuestLoader.QuestLists;
        foreach (Transform child in questSlotRoot)
            Destroy(child.gameObject);

        questSlots.Clear();

        foreach (var quest in questList)
        {
            var slotObj = Instantiate(questSlotPrefab, questSlotRoot);
            var slot = slotObj.GetComponent<QuestSlot>();
            slot.Init(quest, gameManager.DataManager, uIManager, () => { /* 퀘스트 완료 후 행동 */ });
            questSlots.Add(slot);
        }
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
