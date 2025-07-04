using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSelectPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private Button exitBtn;
    [SerializeField] private AssistantSelectTab tabRoot;

    private GameManager gameManager;
    private UIManager uiManager;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        this.gameManager = gameManager;
        this.uiManager = uiManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(ClosePopup);

        if (tabRoot != null)
            tabRoot.Init(gameManager, uiManager);
    }

    public void OpenForSelection(Action<TraineeData> callback)
    {
        if (tabRoot != null)
            tabRoot.OpenForSelection(callback);
    }

    private void ClosePopup()
    {
        if (uiManager != null)
            uiManager.CloseUI(UIName.AssistantSelectPopup);
    }
}
