using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSelectPopup : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private AssistantSelectTab tabRoot;

    private Action<AssistantInstance> onSelectCallback;

    private void Awake()
    {
        if (exitBtn != null)
            exitBtn.onClick.AddListener(ClosePopup);
    }

    public void OpenForSelection(Action<AssistantInstance> callback, bool isMineOrQuestAssign = false)
    {
        onSelectCallback = callback;
        if (tabRoot != null)
            tabRoot.OpenForSelection(OnSelect, isMineOrQuestAssign);
    }

    private void OnSelect(AssistantInstance selected)
    {
        onSelectCallback?.Invoke(selected);
        ClosePopup();
    }

    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
