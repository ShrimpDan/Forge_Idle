using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeInfoPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    private ForgeManager forgeManager;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI fameText;
    [SerializeField] private TextMeshProUGUI sellPriceText;
    [SerializeField] private TextMeshProUGUI autoCraftingText;
    [SerializeField] private TextMeshProUGUI customerSpawnText;
    [SerializeField] private TextMeshProUGUI perfectCraftingText;
    [SerializeField] private TextMeshProUGUI expensiveWeaponText;
    [SerializeField] private TextMeshProUGUI wageText;

    [Header("Exit Button")]
    [SerializeField] private Button exitBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        forgeManager = gameManager.ForgeManager;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.ForgeInfoPopup));
    }

    public override void Open()
    {
        base.Open();

        if (forgeManager.CurrentForge != null)
        {
            SetForgeInfo();
            return;
        }

        uIManager.CloseUI(UIName.ForgeInfoPopup);
    }

    private void SetForgeInfo()
    {
        // 이름 및 레벨 & 명성치
        nameText.text = forgeManager.Name;
        levelText.text = forgeManager.Level.ToString();
        fameText.text = $"(명성치: {forgeManager.TotalFame})";

        ForgeStatHandler statHandler = forgeManager.CurrentForge.StatHandler;

        // 스탯
        sellPriceText.text = $"{statHandler.FinalSellPriceBonus * 100}%";
        autoCraftingText.text = $"{statHandler.FinalAutoCraftingTimeReduction * 100}%";
        customerSpawnText.text = $"{statHandler.FinalCustomerSpawnInterval}s";
        perfectCraftingText.text = $"{statHandler.FinalPerfectCraftingChance}%";
        expensiveWeaponText.text = $"{statHandler.FinalExpensiveWeaponSellChance}%";

        List<AssistantInstance> assiList = gameManager.AssistantInventory.GetAll();

        float wage = 0f;

        foreach (var assi in assiList)
        {
            if (assi.IsFired) continue;
            wage += assi.Wage;   
        }

        // 제자 급여
        wageText.text = UIManager.FormatNumber(wage);
    }
}
