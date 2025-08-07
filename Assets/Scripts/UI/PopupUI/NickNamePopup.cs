using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NickNamePopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancleBtn;

    private string nickName;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(ClickConfirmBtn);

        cancleBtn.onClick.RemoveAllListeners();
        cancleBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.NickNamePopup));
    }

    public void SetNickName(string nickName)
    {
        this.nickName = nickName;
        nickNameText.text = nickName;
    }

    private void ClickConfirmBtn()
    {
        gameManager.ForgeManager.SetNickName(nickName);
        uIManager.CloseUI(UIName.NickNamePopup);
        uIManager.CloseUI(UIName.NickNameWindow);
    }
}
