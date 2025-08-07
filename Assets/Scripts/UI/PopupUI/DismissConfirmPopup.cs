using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissConfirmPopup : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private AssistantTab assistantTab;

    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    private List<AssistantSlot> cachedDismissSlots = new();

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnClickConfirm);
        cancelButton.onClick.AddListener(Close);
    }

    public void Show(List<AssistantSlot> preSelected)
    {
        Debug.Log("[DismissConfirmPopup] Show() 호출됨");

        cachedDismissSlots = new List<AssistantSlot>(preSelected);
        ApplyConfirmButtonColor();
        gameObject.SetActive(true);
    }

    private void OnClickConfirm()
    {
        if (cachedDismissSlots == null || cachedDismissSlots.Count == 0)
        {
            Debug.LogWarning("[DismissConfirmPopup] 선택된 제자가 없어 버튼 동작 무시됨");
            return;
        }

        Debug.Log("<color=cyan>[DismissConfirmPopup] 해고 확인 버튼 눌림</color>");
        var assistantManager = GameManager.Instance.AssistantManager;

        foreach (var slot in cachedDismissSlots)
        {
            var assistant = slot?.Assistant;
            if (assistant == null)
            {
                Debug.LogWarning("[DismissConfirmPopup] 슬롯에 연결된 Assistant가 null입니다.");
                continue;
            }

            assistantManager.DismissAssistant(assistant);
        }

        DismissManager.Instance.ForceResetDismissState();
        assistantTab?.RefreshSlots();
        Close();
    }


    public void Close()
    {
        gameObject.SetActive(false);
        cachedDismissSlots.Clear();
    }

    private void ApplyConfirmButtonColor()
    {
        if (confirmButton == null) return;

        bool hasSelection = cachedDismissSlots != null && cachedDismissSlots.Count > 0;

        Color colorToApply = hasSelection ? selectedColor : defaultColor;

        ColorBlock cb = confirmButton.colors;
        cb.normalColor = colorToApply;
        cb.highlightedColor = colorToApply;
        cb.pressedColor = hasSelection ? new Color(0.9f, 0.9f, 0.9f) : defaultColor;
        cb.selectedColor = colorToApply;
        cb.colorMultiplier = 1;

        confirmButton.colors = cb;
    }

}
