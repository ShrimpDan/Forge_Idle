using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissManager : MonoBehaviour
{
    public static DismissManager Instance { get; private set; }

    [SerializeField] private Button dismissButton;
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

        if (dismissButton != null)
            dismissButton.onClick.AddListener(OnClickDismissButton);
    }

    private void Start()
    {
        ApplyColorByMode();
    }

    private void OnClickDismissButton()
    {
        if (isDismissMode)
        {
            if (selectedSlots.Count == 0)
            {
                isDismissMode = false;
                ApplyColorByMode();

                foreach (var slot in allRegisteredSlots)
                {
                    slot.SetDismissMode(false);
                    slot.SetSelected(false);
                }

                selectedSlots.Clear();
            }
            else
            {
                if (dismissPopup != null)
                    dismissPopup.SetActive(true);
            }
        }
        else
        {
            isDismissMode = true;
            ApplyColorByMode();

            foreach (var slot in allRegisteredSlots)
            {
                slot.SetDismissMode(true);
                slot.SetSelected(false);
            }

            selectedSlots.Clear();
        }
    }


    public void SetDismissMode(bool on)
    {
        isDismissMode = on;
        ApplyColorByMode();

        foreach (var slot in allRegisteredSlots)
        {
            slot.SetDismissMode(on);

            if (!on)
                slot.SetSelected(false);
        }

        selectedSlots.Clear();
    }

    private void ApplyColorByMode()
    {
        if (dismissButton != null)
        {
            ColorBlock cb = dismissButton.colors;

            if (isDismissMode)
            {
                cb.normalColor = Color.white;
                cb.highlightedColor = Color.white;
                cb.pressedColor = new Color(0.9f, 0.9f, 0.9f);
                cb.selectedColor = Color.white;
            }
            else
            {
                cb.normalColor = dismissInactiveColor;
                cb.highlightedColor = dismissInactiveColor;
                cb.pressedColor = dismissInactiveColor;
                cb.selectedColor = dismissInactiveColor;
            }

            cb.colorMultiplier = 1;
            dismissButton.colors = cb;
        }
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
    }

    public void RegisterSlot(AssistantSlot slot)
    {
        if (!allRegisteredSlots.Contains(slot))
            allRegisteredSlots.Add(slot);
    }
}
