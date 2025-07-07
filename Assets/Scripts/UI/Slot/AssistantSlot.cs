using System;
using UnityEngine;
using UnityEngine.UI;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;
    public AssistantData AssistantData { get; private set; }
    [SerializeField] Image icon;
    [SerializeField] Button slotBtn;
    private Action<AssistantData> clickCallback;

    public void Init(AssistantData data, Action<AssistantData> onClick)
    {
        AssistantData = data;
        clickCallback = onClick;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);

        if (icon != null)
            icon.sprite = !string.IsNullOrEmpty(data?.iconPath) ? IconLoader.GetIcon(data.iconPath) : null;

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;
    }

    private void OnClickSlot()
    {
        clickCallback?.Invoke(AssistantData);
        if (AssistantData == null) return;

        var ui = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        if (ui != null)
            ui.SetAssistant(AssistantData);
    }
}
