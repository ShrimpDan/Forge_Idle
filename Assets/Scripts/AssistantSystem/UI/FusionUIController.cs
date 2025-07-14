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

    [Header("합성 후 배경")]
    [SerializeField] private GameObject fusionBackgroundPanel;

    [Header("합성 슬롯")]
    [SerializeField] private Transform slotGroupParent;
    [SerializeField] private GameObject fusionSlotPrefab;

    [Header("합성 중 작동을 중지할 버튼 리스트")]
    [SerializeField] private List<Button> buttonsToDisableDuringFusion;

    [Header("라지 카드 소환 캔버스")]
    [SerializeField] private Transform largeCardParent;

    [Header("합성 상태 텍스트")]
    [SerializeField] private TMP_Text fusionStatusText;

    [Header("전체 합성 버튼")]
    [SerializeField] private Button autoFusionAllButton;

    private readonly List<FusionSlotView> slotViews = new();
    private readonly List<GameObject> currentIcons = new();
    private List<AssistantInstance> fullTraineeList = new();
    private int slotCount = 0;

    private bool isFilteredMode = false;

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

    private static readonly Dictionary<SpecializationType, int> SpecializationOrder = new()
    {
        { SpecializationType.Crafting, 0 },
        { SpecializationType.Enhancing, 1 },
        { SpecializationType.Selling, 2 }
    };


    public void SetFilteredMode(bool isFiltered)
    {
        isFilteredMode = isFiltered;
        autoFusionAllButton.interactable = !isFiltered;
    }

    public void OpenUI(List<AssistantInstance> assistantList)
    {
        fusionUI.SetActive(true);
        fullTraineeList = new List<AssistantInstance>(assistantList);
        ClearAllSlots();
        ClearIcons();
        ShowAllIcons(fullTraineeList);
        SetFilteredMode(false);
    }

    public void CloseUI()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
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

        if (currentTier <= 1) return;

        int newTier = currentTier - 1;

        var personalityCandidates = GameManager.Instance.DataManager.PersonalityLoader.DataList
            .FindAll(p => p.tier == newTier);

        if (personalityCandidates == null || personalityCandidates.Count == 0) return;

        var selectedPersonality = personalityCandidates[Random.Range(0, personalityCandidates.Count)];

        var factory = new AssistantFactory(GameManager.Instance.DataManager);
        AssistantInstance newTrainee = factory.CreateFromSpecAndPersonality(spec, selectedPersonality.Key, newTier);

        if (newTrainee == null)
        {
            Debug.LogWarning("JSON 기반 제자 생성 실패 (CreateFromSpecAndPersonality)");
            return;
        }

        RemoveUsedTrainees();
        HandleFusionResult(newTrainee);
        ResetFusionUIAfterFusion(newTrainee);
        SetButtonsInteractable(true);
    }


    public void PerformAutoFusionAll()
    {
        var inventory = GameManager.Instance.AssistantManager.AssistantInventory;
        var all = inventory.GetAll();
        bool didFusion = false;

        for (int tier = 5; tier >= 2; tier--)
        {
            if (!TierToSlotCount.TryGetValue(tier, out int requiredCount) || requiredCount == 0) continue;

            var grouped = all
                .Where(t => t.Personality.tier == tier)
                .GroupBy(t => t.Specialization)
                .ToList();

            foreach (var group in grouped)
            {
                var assistants = group.ToList();

                while (assistants.Count >= requiredCount)
                {
                    var useList = assistants.Take(requiredCount).ToList();
                    foreach (var t in useList)
                    {
                        inventory.Remove(t);
                        all.Remove(t);
                        assistants.Remove(t);
                    }

                    int newTier = tier - 1;
                    var personalityCandidates = GameManager.Instance.DataManager.PersonalityLoader.DataList
                        .FindAll(p => p.tier == newTier);

                    if (personalityCandidates == null || personalityCandidates.Count == 0) continue;

                    var selectedPersonality = personalityCandidates[Random.Range(0, personalityCandidates.Count)];

                    var factory = new AssistantFactory(GameManager.Instance.DataManager);
                    var newTrainee = factory.CreateFromSpecAndPersonality(group.Key, selectedPersonality.Key, newTier);

                    if (newTrainee == null)
                    {
                        Debug.LogWarning("자동 합성 실패: JSON 기반 제자 없음");
                        continue;
                    }

                    inventory.Add(newTrainee);
                    GameManager.Instance.AssistantManager.ConfirmTrainee(newTrainee);

                    didFusion = true;
                }
            }
        }

        if (didFusion)
        {
            SoundManager.Instance.Play("SFX_CardFusionSuccess");

            fullTraineeList = new List<AssistantInstance>(inventory.GetAll());
            ShowAllIcons(fullTraineeList);
            ClearAllSlots();
            UpdateFusionStatusText();
        }
        else
        {
            SoundManager.Instance.Play("SFX_SystemFail");

            Debug.Log("[FusionUIController] 전체 합성 실패: 인벤토리에 합성 가능한 제자가 부족합니다.");
        }
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
        SetFilteredMode(false);
        UpdateFusionStatusText();
    }

    private void SetSlotData(int index, AssistantInstance data)
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

    private void ShowAllIcons(List<AssistantInstance> list)
    {
        ClearIcons();

        HashSet<AssistantInstance> used = slotViews
            .Where(slot => slot.Data != null)
            .Select(slot => slot.Data)
            .ToHashSet();

        var sortedList = list
            .Where(t =>
                !used.Contains(t) &&
                t.Personality.tier >= 2 &&
                !t.IsEquipped &&
                !t.IsInUse)
            .OrderBy(t => SpecializationOrder.TryGetValue(t.Specialization, out int order) ? order : 99)
            .ThenBy(t => t.Personality.tier)
            .ToList();

        foreach (var assistant in sortedList)
        {
            GameObject icon = Instantiate(assistantIconPrefab, iconParent);
            icon.transform.SetParent(iconParent, false);
            icon.transform.localScale = Vector3.one;
            icon.transform.localRotation = Quaternion.identity;
            icon.transform.localPosition = Vector3.zero;

            icon.GetComponent<AssistantIconView>().Init(assistant, () => OnTraineeIconClicked(assistant));
            currentIcons.Add(icon);
        }

        ResizeContent(currentIcons.Count);
        LayoutRebuilder.ForceRebuildLayoutImmediate(iconParent.GetComponent<RectTransform>());

        if (scrollRect != null)
            StartCoroutine(ForceScrollToTop());
    }



    private void ShowFilteredIcons(AssistantInstance reference)
    {
        if (reference == null)
        {
            ShowAllIcons(fullTraineeList);
            return;
        }

        List<AssistantInstance> filtered = fullTraineeList
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
            {
                GameManager.Instance.AssistantManager.AssistantInventory.Remove(slot.Data);
            }
    }

    private void HandleFusionResult(AssistantInstance fused)
    {
        GameManager.Instance.AssistantManager.AssistantInventory.Add(fused);
        GameManager.Instance.AssistantManager.ConfirmTrainee(fused);

        if (fusionBackgroundPanel != null)
            fusionBackgroundPanel.SetActive(true);

        GameObject card = GameManager.Instance.AssistantManager.Spawner
            .SpawnLargeCard(fused, largeCardParent, OnFusionCardConfirmed, true, true);
    }

    private void OnFusionCardConfirmed(AssistantInstance data)
    {

        if (fusionBackgroundPanel != null)
            fusionBackgroundPanel.SetActive(false);

        if (fusionStatusText != null)
            fusionStatusText.gameObject.SetActive(false);

        ClearAllSlots();
    }

    private void ResetFusionUIAfterFusion(AssistantInstance newTrainee)
    {
        var inventory = GameManager.Instance.AssistantManager.AssistantInventory;
        fullTraineeList = new List<AssistantInstance>(inventory.GetAll());

        foreach (var slot in slotViews)
            slot.Clear();

        ShowAllIcons(fullTraineeList);
        SetFilteredMode(false);
        UpdateFusionStatusText();
    }

    private void OnTraineeIconClicked(AssistantInstance data)
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

        SetFilteredMode(true);

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

    public List<FusionSlotView> GetCurrentSlots() => slotViews;

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

    public void RefreshUIFromInventory()
    {
        var inventory = GameManager.Instance.AssistantManager.AssistantInventory;
        fullTraineeList = new List<AssistantInstance>(inventory.GetAll());
        ShowAllIcons(fullTraineeList);
        ClearAllSlots();
        UpdateFusionStatusText();
    }
}

