using UnityEngine;
using UnityEngine.UI;

public class ForgeMoveWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    private ForgeManager forgeManager;

    [SerializeField] ForgeMoveSlot[] forgeMoveSlots;
    [SerializeField] Button exitBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forgeManager = gameManager.ForgeManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.ForgeMoveWindow));
    }

    public override void Open()
    {
        base.Open();

        foreach (var slot in forgeMoveSlots)
        {
            slot.SetSlot(forgeManager);
        }
    }
}
