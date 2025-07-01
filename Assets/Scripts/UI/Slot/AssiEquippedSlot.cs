using UnityEngine;
using UnityEngine.UI;

public class AssiEquippedSlot : MonoBehaviour
{
    private UIManager uIManager;

    public TraineeData EquippedAssi { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;

    public void Init(UIManager uIManager)
    {
        this.uIManager = uIManager;

        slotBtn = GetComponent<Button>();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);
    }

    public void SetAssistant(TraineeData assi)
    {
        if (assi == null)
        {
            UnEquipAssistant();
            return;
        }
        
        EquippedAssi = assi;
        //icon.sprite = IconLoader.GetIcon(assi.IconPath);
    }
    
    private void OnClickSlot()
    {
        if (EquippedAssi == null) return;

        var ui = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        ui.SetAssistant(EquippedAssi);
    }

    public void UnEquipAssistant()
    {
        EquippedAssi = null;
        icon.sprite = null;
    }
}
