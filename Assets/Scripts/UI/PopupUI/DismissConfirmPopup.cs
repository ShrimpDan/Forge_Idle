using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DismissConfirmPopup : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private AssistantTab assistantTab;

    private List<AssistantSlot> cachedDismissSlots = new();

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnClickConfirm);
        cancelButton.onClick.AddListener(Close);
    }

    public void Show(List<AssistantSlot> preSelected)
    {
        Debug.Log("[DismissConfirmPopup] Show() 호출됨");

        cachedDismissSlots = new List<AssistantSlot>(preSelected);
        gameObject.SetActive(true);
    }

    private void OnClickConfirm()
    {
        Debug.Log("<color=cyan>[DismissConfirmPopup] 해고 확인 버튼 눌림</color>");
        var assistantManager = GameManager.Instance.AssistantManager;

        Debug.Log($"<color=cyan>[DismissConfirmPopup] 캐싱된 슬롯 수: {cachedDismissSlots.Count}</color>");

        foreach (var slot in cachedDismissSlots)
        {
            var assistant = slot?.Assistant;
            if (assistant == null)
            {
                Debug.LogWarning("[DismissConfirmPopup] 슬롯에 연결된 Assistant가 null입니다.");
                continue;
            }

            Debug.Log($"<color=yellow>[DismissConfirmPopup] 해고 대상: {assistant.Name}</color>");
            assistantManager.DismissAssistant(assistant);
        }

        DismissManager.Instance.SetDismissMode(false);
        assistantTab?.RefreshSlots();
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        cachedDismissSlots.Clear();
    }
}
