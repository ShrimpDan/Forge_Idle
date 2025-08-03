using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WagePopup : MonoBehaviour
{
    [SerializeField] private AssistantTab assistantTab;

    [SerializeField] private TMP_Text wageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void Show()
    {
        int totalWage = 0;

        var trainees = GameManager.Instance.AssistantInventory?.GetActiveTrainees();
        if (trainees != null)
        {
            foreach (var trainee in trainees)
                totalWage += trainee.Wage;
        }

        wageText.text = $"{UIManager.FormatNumber(totalWage)}";
    }

    public void Close()
    {
        gameObject.SetActive(false);
        transform.parent.Find("DimBackground")?.gameObject.SetActive(false);

        DismissManager.Instance?.SetDismissMode(false);

        assistantTab?.RefreshSlots();
    }
}
