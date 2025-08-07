using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NickNameWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    public TMP_InputField inputField;
    public TextMeshProUGUI warningText;
    public Button confirmBtn;

    // 로직 스크립트 참조
    private NicknameValidator validator;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        
        validator = GetComponent<NicknameValidator>();

        confirmBtn.onClick.AddListener(OnConfirmButtonClicked);
        inputField.onValueChanged.AddListener(OnInputValueChanged);

        confirmBtn.interactable = false;
        warningText.text = "";
    }

    private void OnInputValueChanged(string input)
    {
        confirmBtn.interactable = input != null;
    }

    private bool OnCheckButtonClicked()
    {
        string nickname = inputField.text;
        string resultMessage;
        bool isValid = validator.IsValidNickName(nickname, out resultMessage);

        warningText.text = resultMessage;
        warningText.color = isValid ? Color.green : Color.red;

        return isValid;
    }

    private void OnConfirmButtonClicked()
    {
        if (OnCheckButtonClicked())
        {
            var ui = uIManager.OpenUI<NickNamePopup>(UIName.NickNamePopup);
            ui.SetNickName(inputField.text);
            return;
        }

        return;
    }
}
