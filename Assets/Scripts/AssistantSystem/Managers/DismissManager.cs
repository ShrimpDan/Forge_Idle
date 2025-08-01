using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DismissManager : MonoBehaviour
{
    public static DismissManager Instance { get; private set; }

    [Header("Dismiss UI")]
    [SerializeField] private GameObject dismissPopup;
    [SerializeField] private Transform slotParent;

    [Header("Dismiss Button & Color")]
    [SerializeField] private Button dismissButton;
    [SerializeField] private Color dismissActiveColor = Color.white;
    [SerializeField] private Color dismissInactiveColor = new Color(1f, 1f, 1f, 0.4f);

    private AssistantManager assistantManager;
    private AssistantSaveHandler saveHandler;

    private List<AssistantSlot> allSlots = new List<AssistantSlot>();
    private HashSet<AssistantSlot> selectedSlots = new HashSet<AssistantSlot>();

    private bool isDismissMode = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UpdateDismissButtonColor();
    }

    public void Init(AssistantManager manager, AssistantSaveHandler saveHandler)
    {
        this.assistantManager = manager;
        this.saveHandler = saveHandler;
        dismissPopup.SetActive(false);

        ExitDismissMode();
    }

    public void EnterDismissMode()
    {
        isDismissMode = true;
        selectedSlots.Clear();

        foreach (var slot in allSlots)
        {
            slot.SetDismissMode(true);
            slot.SetSelectedForDismiss(false);
        }

        UpdateDismissButtonColor();
    }

    public void ExitDismissMode()
    {
        isDismissMode = false;
        selectedSlots.Clear();

        foreach (var slot in allSlots)
        {
            slot.SetDismissMode(false);
            slot.SetSelectedForDismiss(false);
        }

        dismissPopup.SetActive(false);
        UpdateDismissButtonColor();
    }

    public void ToggleDismissMode()
    {
        if (isDismissMode)
            ExitDismissMode();
        else
            EnterDismissMode();
    }

    private void UpdateDismissButtonColor()
    {
        if (dismissButton != null)
        {
            dismissButton.image.color = isDismissMode ? dismissActiveColor : dismissInactiveColor;
        }
    }

    public void RegisterSlot(AssistantSlot slot)
    {
        if (!allSlots.Contains(slot))
        {
            allSlots.Add(slot);

            slot.OnClicked += () =>
            {
                if (!isDismissMode) return;

                if (selectedSlots.Contains(slot))
                {
                    DeselectSlot(slot);
                    slot.SetSelectedForDismiss(false);
                }
                else
                {
                    SelectSlot(slot);
                    slot.SetSelectedForDismiss(true);
                }
            };
        }
    }

    public void OnClickDismissConfirm()
    {
        if (selectedSlots.Count == 0)
        {
            Debug.Log("[DismissManager] 해고할 제자를 선택해주세요.");
            return;
        }

        dismissPopup.SetActive(true);
    }

    public void OnClickDismissCancel()
    {
        dismissPopup.SetActive(false);
        ExitDismissMode();
    }

    public void OnClickDismissConfirmYes()
    {
        foreach (var slot in selectedSlots.ToList())
        {
            AssistantInstance assistant = slot.Assistant;
            assistantManager.AssistantInventory.Remove(assistant);
        }

        saveHandler.Save();
        ExitDismissMode();
    }

    public bool IsDismissMode()
    {
        return isDismissMode;
    }

    public void SelectSlot(AssistantSlot slot)
    {
        if (slot != null)
            selectedSlots.Add(slot);
    }

    public void DeselectSlot(AssistantSlot slot)
    {
        if (slot != null)
            selectedSlots.Remove(slot);
    }
}
