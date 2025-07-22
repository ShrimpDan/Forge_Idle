using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecruitConfirmPopup : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void Show(string message, System.Action onConfirm, System.Action onCancel)
    {
        root.SetActive(true);
        messageText.text = message;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Hide();
        });

        cancelButton.onClick.AddListener(() =>
        {
            onCancel?.Invoke();
            Hide();
        });
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
