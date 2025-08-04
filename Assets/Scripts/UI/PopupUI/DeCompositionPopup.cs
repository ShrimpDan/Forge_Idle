using UnityEngine;
using UnityEngine.UI;

public class DecompositionPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    private DecompositionWindow decompositionWindow;

    [SerializeField] Button confirmButton;
    [SerializeField] Button cancleButton;


    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(ClickConfirmButton);

        cancleButton.onClick.RemoveAllListeners();
        cancleButton.onClick.AddListener(() => uIManager.CloseUI(UIName.DecompositionPopup));
    }

    public void SetUI(DecompositionWindow decompositionWindow)
    {
        this.decompositionWindow = decompositionWindow;
    }

    private void ClickConfirmButton()
    {
        decompositionWindow.DecompositionWeapons();
        uIManager.CloseUI(UIName.DecompositionPopup);
    }
}
