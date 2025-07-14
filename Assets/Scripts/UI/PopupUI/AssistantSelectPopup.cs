using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSelectPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;
    [SerializeField] private Button exitBtn;
    [SerializeField] private AssistantSelectTab tabRoot;

    private UIManager uiManager;
    private Action<AssistantInstance> onSelectCallback;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(ClosePopup);
        tabRoot?.Init(gameManager, uiManager);
    }

    public void OpenForSelection(Action<AssistantInstance> callback, bool isMineOrQuestAssign = false)
    {
        onSelectCallback = callback;
        if (tabRoot != null)
            tabRoot.OpenForSelection(onSelectCallback, isMineOrQuestAssign);
    }

    private void ClosePopup()
    {
        if (uiManager != null)
            uiManager.CloseUI(UIName.AssistantSelectPopup);
    }
}
