using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAssistantData
{
    public string IconPath;
    public string TypeIconPath;
    public string Type;
    public string Name;
    public List<string> OptionList;
}

public class AssistantSlot : MonoBehaviour
{
    public TraineeData AssistantData { get; private set; }

    [SerializeField] Image icon;
    [SerializeField] Button slotBtn;

    public void Init(TraineeData data)
    {
        AssistantData = data;
        slotBtn.onClick.AddListener(OpenPopup);

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(() => OnClickSlot());

        //icon.sprite = Resources.Load<Sprite>(data);
    }

    private void OnClickSlot()
    {
        if (AssistantData == null) return;

        var ui = UIManager.Instance.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        ui.SetAssistant(AssistantData);
    }

    private void OpenPopup()
    {
        


    }
}
