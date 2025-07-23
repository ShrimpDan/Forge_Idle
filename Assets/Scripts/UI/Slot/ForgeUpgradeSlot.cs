using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeUpgradeSlot : MonoBehaviour
{
    private ForgeUpgradeWindow upgradeWindow;


    [SerializeField] private ForgeUpgradeType type;
    public ForgeUpgradeType UpgradeType { get => type; }

    [Header("UI Elemets")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeBtn;

    public void Init(ForgeUpgradeWindow upgradeWindow)
    {
        this.upgradeWindow = upgradeWindow;

        upgradeBtn.onClick.RemoveAllListeners();
        upgradeBtn.onClick.AddListener(Upgrade);
    }

    public void SetSlot(int level, float curValue, float nextValue, int cost)
    {
        levelText.text = $"Lv.{level}";
        costText.text = UIManager.FormatNumber(cost);
        upgradeBtn.interactable = true;

        if (type == ForgeUpgradeType.ReduceCustomerSpawnDelay)
        {
            valueText.text = $"{curValue}s -> {nextValue}s";
        }
        else if (type == ForgeUpgradeType.UpgradeInterior)
        {
            valueText.text = $"{Mathf.RoundToInt(curValue)} -> {Mathf.RoundToInt(nextValue)}";
        }
        else
        {
            valueText.text = $"{curValue * 100}% -> {nextValue * 100}%";
        }
    }

    public void SetSlot(int level, float curValue)
    {
        levelText.text = $"Lv.{level}";
        costText.text = "최대";
        upgradeBtn.interactable = false;

        if (type == ForgeUpgradeType.ReduceCustomerSpawnDelay)
        {
            valueText.text = $"{curValue}s";
        }
        else if (type == ForgeUpgradeType.UpgradeInterior)
        {
            valueText.text = $"{Mathf.RoundToInt(curValue)}";
        }
        else
        {
            valueText.text = $"{curValue * 100}%";
        }
    }

    private void Upgrade()
    {
        upgradeWindow.UpgradeForge(this);
    }
}
