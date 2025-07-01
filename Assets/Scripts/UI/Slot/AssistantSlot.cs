using System;
using UnityEngine;
using UnityEngine.UI;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;
    public TraineeData AssistantData { get; private set; }
    [SerializeField] Image icon;
    [SerializeField] Button slotBtn;
    private Action<TraineeData> clickCallback;

    // 콜백 버전 Init!
    public void Init(TraineeData data, Action<TraineeData> onClick)
    {
        AssistantData = data;
        clickCallback = onClick;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);

        //icon.sprite = IconLoader.GetIcon()

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
