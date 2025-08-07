using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [Header("UI Elements")]
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI rarityText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Button exitBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CollectionPopup));
    }

    public void SetPopup(RegularCustomerData data) //데이터 넘겨 받는곳
    {
        icon.sprite = IconLoader.GetIconByPath(data.iconPath);
        nameText.text = data.customerName;
        rarityText.text = GetStringByRarity(data.rarity);
        descText.text = data.customerInfo;
    }

    private string GetStringByRarity(CustomerRarity rarity)
    {
        return rarity switch
        {
            CustomerRarity.Common => "일반",
            CustomerRarity.Rare => "희귀",
            CustomerRarity.Epic => "에픽",
            CustomerRarity.Unique => "유니크",
            CustomerRarity.Legendary => "전설",
            _ => string.Empty
        };
    }
}
