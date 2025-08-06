using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text wageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void Show()
    {
        Debug.Log("[WagePopup] Show() 호출됨");

        int totalWage = 0;
        var trainees = GameManager.Instance.AssistantInventory?.GetActiveTrainees();
        if (trainees != null)
        {
            foreach (var trainee in trainees)
                totalWage += trainee.Wage;
        }

        wageText.text = $"{UIManager.FormatNumber(totalWage)}/분";
    }

    private void Close()
    {
        gameObject.SetActive(false);
        transform.parent.Find("DimBackground")?.gameObject.SetActive(false);
    }
}
