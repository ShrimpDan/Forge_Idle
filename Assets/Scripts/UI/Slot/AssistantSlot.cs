using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;
    public TraineeData AssistantData { get; private set; }
    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;
    private Action<TraineeData> clickCallback;

    public void Init(TraineeData data, Action<TraineeData> onClick)
    {
        AssistantData = data;
        clickCallback = onClick;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);

        if (icon != null)
        {
            string iconPath = data?.IconPath;
            icon.sprite = !string.IsNullOrEmpty(iconPath)
                ? IconLoader.GetIcon(iconPath)
                : null;
            icon.enabled = icon.sprite != null;
        }

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;
    }

    private void OnClickSlot()
    {
        clickCallback?.Invoke(AssistantData);
        if (AssistantData == null) return;

        var ui = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        ui.SetAssistant(AssistantData);
    }
}
