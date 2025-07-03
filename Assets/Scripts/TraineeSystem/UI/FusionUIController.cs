using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FusionUIController : MonoBehaviour
{
    [Header("합성 UI")]
    [SerializeField] private GameObject fusionUI;
    [SerializeField] private Transform iconParent;
    [SerializeField] private GameObject assistantIconPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("합성 슬롯")]
    [SerializeField] private Transform slotGroupParent;
    [SerializeField] private GameObject fusionSlotPrefab;

    [Header("합성 중 작동을 중지할 버튼 리스트")]
    [SerializeField] private List<Button> buttonsToDisableDuringFusion;

    [Header("라지 카드 소환 캔버스")]
    [SerializeField] private Transform largeCardParent;

    [Header("합성 상태 텍스트")]
    [SerializeField] private TMP_Text fusionStatusText;

    private readonly List<FusionSlotView> slotViews = new();
    private readonly List<GameObject> currentIcons = new();
    private List<TraineeData> fullTraineeList = new();
    private int slotCount = 0;

    private static readonly Dictionary<int, int> TierToSlotCount = new()
    {
        { 5, 5 }, { 4, 4 }, { 3, 3 }, { 2, 2 }
    };

    private static readonly Dictionary<int, Vector2> TierToSlotSize = new()
    {
        { 5, new Vector2(180, 180) },
        { 4, new Vector2(240, 240) },
        { 3, new Vector2(280, 280) },
        { 2, new Vector2(400, 400) }
    };

    public void OpenUI(List<TraineeData> traineeList)
    {
        fusionUI.SetActive(true);
        fullTraineeList = new List<TraineeData>(traineeList);
        ClearAllSlots();
        ClearIcons();
        ShowAllIcons(fullTraineeList);
    }

    public void CloseUI()
    {
        if (slotViews.Count > 0 && slotViews[0].Data != null)
        {
            ClearAllSlotsAndReset();
            return;
        }
        fusionUI.SetActive(false);
    }

    public void OnClick_FusionButton()
    {
        if (slotViews.Any(slot => slot.Data == null)) return;

        SpecializationType spec = slotViews[0].Data.Specialization;
        int currentTier = slotViews[0].Data.Personality.tier;

        if (currentTier <= 1)
        {
            Debug.LogWarning("이미 최상위 티어입니다.");
            return;
        }

        int newTier = currentTier - 1;
        List<PersonalityData> candidates = GameManager.Instance.DataManager.PersonalityLoader.DataList
            .FindAll(p => p.tier == newTier);

        if (candidates == null || candidates.Count == 0)
        {
            Debug.LogError($"티어 {newTier}에 해당하는 성격이 없습니다.");
            return;
        }

        RemoveUsedTrainees();
        PersonalityData selected = candidates[Random.Range(0, candidates.Count)];
        var assigner = new PersonalityAssigner(GameManager.Instance.DataManager);
        var multipliers = assigner.GenerateMultipliers(selected, spec);

        string name = $"합성제자_{spec}_{selected.personalityName}";
        TraineeData newTrainee = new(name, selected, spec, multipliers, 1, false, false);

        HandleFusionResult(newTrainee);
        ResetFusionUIAfterFusion(newTrainee);
        SetButtonsInteractable(true);
    }

    private void ConfigureFusionSlots(int tier)
    {
        ClearAllSlots();

        slotCount = TierToSlotCount.TryGetValue(tier, out int count) ? count : 0;
        Vector2 slotSize = TierToSlotSize.TryGetValue(tier, out var size) ? size : new Vector2(180, 180);

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(fusionSlotPrefab, slotGroupParent);
            slotObj.name = $"FusionSlot_{i + 1}";
            if (slotObj.TryGetComponent(out RectTransform rt))
                rt.sizeDelta = slotSize;

            var view = slotObj.GetComponent<FusionSlotView>();
            view.Clear();
            slotViews.Add(view);
        }
    }

    private void ClearAllSlots()
    {
        foreach (Transform child in slotGroupParent)
            Destroy(child.gameObject);
        slotViews.Clear();
    }

    private void ClearAllSlotsAndReset()
    {
        foreach (var slot in slotViews)
            slot.Clear();
        ShowAllIcons(fullTraineeList);
        UpdateFusionStatusText();
    }

    private void SetSlotData(int index, TraineeData data)
    {
        if (index < 0 || index >= slotViews.Count) return;
        slotViews[index].SetData(data, GetSlotClickAction(index));
        UpdateFusionStatusText();
    }

    private System.Action GetSlotClickAction(int index)
    {
        return index == 0 ? ClearAllSlotsAndReset : () => RemoveSlotAndShift(index);
    }

    private void RemoveSlotAndShift(int index)
    {
        if (index < 1 || index >= slotViews.Count) return;

        for (int i = index; i < slotViews.Count - 1; i++)
        {
            slotViews[i].SetData(slotViews[i + 1].Data, slotViews[i + 1].Data != null ? GetSlotClickAction(i) : null);
        }

        slotViews[^1].Clear();

        if (slotViews[0].Data != null)
            ShowFilteredIcons(slotViews[0].Data);

        UpdateFusionStatusText();
    }

    private void ShowAllIcons(List<TraineeData> list)
    {
        ClearIcons();

        HashSet<TraineeData> used = slotViews
            .Where(slot => slot.Data != null)
            .Select(slot => slot.Data)
            .ToHashSet();

        var sortedList = list
            .Where(t => !used.Contains(t) && t.Personality.tier >= 2)
            .OrderBy(t => SpecializationOrder.TryGetValue(t.Specialization, out int order) ? order : 99)
            .ThenBy(t => t.Personality.tier)
            .ToList();

        foreach (var trainee in sortedList)
        {
            GameObject icon = Instantiate(assistantIconPrefab, iconParent);
            icon.transform.SetParent(iconParent, false);
            icon.transform.localScale = Vector3.one;
            icon.transform.localRotation = Quaternion.identity;
            icon.transform.localPosition = Vector3.zero;

            icon.GetComponent<AssistantIconView>().Init(trainee, () => OnTraineeIconClicked(trainee));
            currentIcons.Add(icon);
        }

        ResizeContent(currentIcons.Count);
        LayoutRebuilder.ForceRebuildLayoutImmediate(iconParent.GetComponent<RectTransform>());

        if (scrollRect != null)
            StartCoroutine(ForceScrollToTop());
    }

    private void ShowFilteredIcons(TraineeData reference)
    {
        if (reference == null)
        {
            ShowAllIcons(fullTraineeList);
            return;
        }

        List<TraineeData> filtered = fullTraineeList
            .FindAll(t =>
                t.Personality.tier >= 2 &&
                !slotViews.Exists(slot => slot.Data == t) &&
                t.Specialization == reference.Specialization &&
                t.Personality.tier == reference.Personality.tier)
            .OrderBy(t => SpecializationOrder.TryGetValue(t.Specialization, out int order) ? order : 99)
            .ThenBy(t => t.Personality.tier)
            .ToList();

        ShowAllIcons(filtered);
    }

    private void ClearIcons()
    {
        foreach (Transform child in iconParent)
            Destroy(child.gameObject);
        currentIcons.Clear();
    }

    private IEnumerator ForceScrollToTop()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void ResizeContent(int itemCount)
    {
        var grid = iconParent.GetComponent<GridLayoutGroup>();
        var rt = iconParent.GetComponent<RectTransform>();
        if (grid == null || rt == null) return;

        int columns = grid.constraintCount;
        int paddedItemCount = Mathf.Max(itemCount, 0);

        if (paddedItemCount % columns != 0)
            paddedItemCount += columns - (paddedItemCount % columns);

        int rows = Mathf.CeilToInt(paddedItemCount / (float)columns);
        rows = Mathf.Max(rows, 4);

        float totalHeight = grid.padding.top + grid.padding.bottom + (grid.cellSize.y + grid.spacing.y) * rows - grid.spacing.y;
        totalHeight += 1200f;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalHeight);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    public void RemoveUsedTrainees()
    {
        foreach (var slot in slotViews)
            if (slot.Data != null)
                GameManager.Instance.AssistantManager.TraineeInventory.Remove(slot.Data);
    }

    private void HandleFusionResult(TraineeData fused)
    {
        GameManager.Instance.AssistantManager.TraineeInventory.Add(fused);
        GameManager.Instance.AssistantManager.ConfirmTrainee(fused);

        GameObject card = GameManager.Instance.AssistantManager.Spawner
            .SpawnLargeCard(fused, largeCardParent, null, true, true);
    }

    private void ResetFusionUIAfterFusion(TraineeData newTrainee)
    {
        fullTraineeList.Add(newTrainee);
        foreach (var slot in slotViews) slot.Clear();
        ShowAllIcons(fullTraineeList);
    }

    private void OnTraineeIconClicked(TraineeData data)
    {
        if (slotViews.Count == 0 || slotViews[0].Data == null)
        {
            ConfigureFusionSlots(data.Personality.tier);
            SetSlotData(0, data);
        }
        else
        {
            for (int i = 1; i < slotViews.Count; i++)
            {
                if (slotViews[i].Data == null)
                {
                    SetSlotData(i, data);
                    break;
                }
            }
        }

        ShowFilteredIcons(slotViews[0].Data);
        UpdateFusionStatusText();
    }

    private void UpdateFusionStatusText()
    {
        if (slotViews.Count == 0 || slotViews[0].Data == null)
        {
            fusionStatusText.gameObject.SetActive(false);
            return;
        }

        var reference = slotViews[0].Data;
        int currentTier = reference.Personality.tier;
        string specKor = GetSpecKorean(reference.Specialization);

        fusionStatusText.text = $"[{specKor}] : {currentTier}티어 합성 진행 중";
        fusionStatusText.gameObject.SetActive(true);
    }

    private string GetSpecKorean(SpecializationType type)
    {
        return type switch
        {
            SpecializationType.Crafting => "제작 특화",
            SpecializationType.Enhancing => "강화 특화",
            SpecializationType.Selling => "판매 특화",
            _ => "알 수 없음"
        };
    }

    public void SetButtonsInteractable(bool interactable)
    {
        foreach (var btn in buttonsToDisableDuringFusion)
            if (btn != null) btn.interactable = interactable;
    }

    public void AddSlotButtonsToDisableList()
    {
        foreach (var slot in slotViews)
        {
            var btn = slot.GetComponent<Button>();
            if (btn != null && !buttonsToDisableDuringFusion.Contains(btn))
                buttonsToDisableDuringFusion.Add(btn);
        }
    }
    private static readonly Dictionary<SpecializationType, int> SpecializationOrder = new()
{
    { SpecializationType.Crafting, 0 },
    { SpecializationType.Enhancing, 1 },
    { SpecializationType.Selling, 2 }
};

    public List<FusionSlotView> GetCurrentSlots() => slotViews;
}
