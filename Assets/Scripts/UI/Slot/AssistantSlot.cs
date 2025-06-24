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
    public TestAssistantData AssistantData { get; private set; }

    [SerializeField] Image icon;
    [SerializeField] Button slotBtn;

    public void Init(TestAssistantData data)
    {
        AssistantData = data;
        slotBtn.onClick.AddListener(OpenPopup);
    }

    private void OpenPopup()
    {

    }
}
