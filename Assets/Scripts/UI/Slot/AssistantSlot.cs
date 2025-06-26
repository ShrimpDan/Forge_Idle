using UnityEngine;
using UnityEngine.UI;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;

    public TraineeData AssistantData { get; private set; }

    [SerializeField] Image icon;
    [SerializeField] Button slotBtn;

    public void Init(TraineeData data)
    {
        AssistantData = data;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);

        //icon.sprite = IconLoader.GetIcon()

        if(uIManager == null)
            uIManager = GameManager.Instance.UIManager;
    }

    private void OnClickSlot()
    {
        if (AssistantData == null) return;

        var ui = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        ui.SetAssistant(AssistantData);
    }
}
