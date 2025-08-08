using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissManager : MonoBehaviour
{
    public static DismissManager Instance { get; private set; }

    [SerializeField] private Button toggleDismissModeButton;
    [SerializeField] private Button confirmDismissButton;
    [SerializeField] private Color dismissActiveColor = Color.white;
    [SerializeField] private Color dismissInactiveColor = new Color32(200, 200, 200, 255);
    [SerializeField] private GameObject dismissPopup;

    private bool isDismissMode = false;

    private HashSet<AssistantSlot> selectedSlots = new();
    private List<AssistantSlot> allRegisteredSlots = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (toggleDismissModeButton != null)
            toggleDismissModeButton.onClick.AddListener(ToggleDismissMode);

        if (confirmDismissButton != null)
            confirmDismissButton.onClick.AddListener(OnClickConfirmDismiss);
    }

    private void Start()
    {
        ApplyToggleButtonColor();
        UpdateConfirmButtonState();
    }
    
    private void ToggleDismissMode()
    {
        SetDismissMode(!isDismissMode);

        Debug.Log($"[DismissManager] 해고 모드 {(isDismissMode ? "진입" : "종료")}");

        foreach (var slot in allRegisteredSlots)
        {
            slot.SetDismissMode(isDismissMode);
            slot.SetSelected(false);
        }

        selectedSlots.Clear();
        UpdateConfirmButtonState();
    }

    private void OnClickConfirmDismiss()
    {
        if (selectedSlots.Count == 0) return;

        Debug.Log($"[DismissManager] 해고 대상 {selectedSlots.Count}명 → 확인 팝업 열림");

        foreach (var slot in selectedSlots)
        {
            Debug.Log($" - 선택된 제자: {slot.Assistant?.Name ?? "null"}");
        }

        if (dismissPopup != null)
        {
            var popup = dismissPopup.GetComponent<DismissConfirmPopup>();
            if (popup != null)
            {
                popup.Show(GetSelectedSlotsCopy());
            }
            else
            {
                Debug.LogWarning("[DismissManager] dismissPopup에 DismissConfirmPopup 컴포넌트가 없습니다.");
            }

            dismissPopup.SetActive(true);
        }
    }

    public void SetDismissMode(bool on)
    {
        isDismissMode = on;
        ApplyToggleButtonColor();
        UpdateConfirmButtonState();

        foreach (var slot in allRegisteredSlots)
        {
            slot.SetDismissMode(on);

            if (!on)
                slot.SetSelected(false);
        }

        selectedSlots.Clear();
    }

    private void ApplyToggleButtonColor()
    {
        if (toggleDismissModeButton == null) return;

        ColorBlock cb = toggleDismissModeButton.colors;

        if (isDismissMode)
        {
            cb.normalColor = dismissActiveColor;
            cb.highlightedColor = dismissActiveColor;
            cb.pressedColor = new Color(0.9f, 0.9f, 0.9f);
            cb.selectedColor = dismissActiveColor;
        }
        else
        {
            cb.normalColor = dismissInactiveColor;
            cb.highlightedColor = dismissInactiveColor;
            cb.pressedColor = dismissInactiveColor;
            cb.selectedColor = dismissInactiveColor;
        }

        cb.colorMultiplier = 1;
        toggleDismissModeButton.colors = cb;
    }

    private void UpdateConfirmButtonState()
    {
        if (confirmDismissButton == null) return;

        confirmDismissButton.gameObject.SetActive(isDismissMode);

        bool hasSelection = selectedSlots.Count > 0;
        confirmDismissButton.interactable = hasSelection;

        var cb = confirmDismissButton.colors;
        var red = new Color(1f, 0.2f, 0.2f, 1f);
        cb.normalColor = red;
        cb.highlightedColor = red;
        cb.pressedColor = red * 0.9f;
        cb.selectedColor = red;
        cb.disabledColor = red;
        cb.colorMultiplier = 1f;
        confirmDismissButton.colors = cb;
    }

    public bool IsDismissMode() => isDismissMode;

    public void ToggleSelect(AssistantSlot slot)
    {
        if (!isDismissMode) return;

        if (selectedSlots.Contains(slot))
        {
            selectedSlots.Remove(slot);
            slot.SetSelected(false);
        }
        else
        {
            selectedSlots.Add(slot);
            slot.SetSelected(true);
        }

        UpdateConfirmButtonState();
    }

    public void RegisterSlot(AssistantSlot slot)
    {
        if (!allRegisteredSlots.Contains(slot))
            allRegisteredSlots.Add(slot);
    }

    public IEnumerable<AssistantSlot> GetSelectedSlots()
    {
        return selectedSlots;
    }

    public List<AssistantInstance> GetSelectedAssistants()
    {
        List<AssistantInstance> result = new();

        foreach (var slot in selectedSlots)
        {
            if (slot == null)
            {
                Debug.LogWarning("[DismissManager] 선택된 슬롯 중 null 있음");
                continue;
            }

            var assistant = slot.Assistant;
            if (assistant == null)
            {
                Debug.LogWarning("[DismissManager] 슬롯에 AssistantInstance가 null입니다");
                continue;
            }

            result.Add(assistant);
        }

        return result;
    }

    public List<AssistantSlot> GetSelectedSlotsCopy()
    {
        return new List<AssistantSlot>(selectedSlots);
    }

    public void LogCurrentSelected()
    {
        Debug.Log($"[DismissManager] 현재 선택된 슬롯 수: {selectedSlots.Count}");

        foreach (var slot in selectedSlots)
        {
            Debug.Log($" - 슬롯: {slot.name}, 어시스턴트: {slot.Assistant?.Name}, isSelected={slot.IsSelected()}");
        }
    }

    public void ForceResetDismissState()
    {
        isDismissMode = false;

        foreach (var slot in allRegisteredSlots)
        {
            slot.SetDismissMode(false);
            slot.SetSelected(false);
        }

        selectedSlots.Clear();

        ApplyToggleButtonColor();
        UpdateConfirmButtonState();
    }

    public void ResetDismissButton()
    {
        ApplyToggleButtonColor();
    }
}
