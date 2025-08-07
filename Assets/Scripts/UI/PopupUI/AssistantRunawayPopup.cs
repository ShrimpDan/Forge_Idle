using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistantRunawayPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Button confirmButton;

    private List<GameObject> spawnedIcons = new();

    private void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnClickConfirm);
    }

    /// <summary>
    /// 탈주한 제자들을 받아 팝업을 표시합니다.
    /// </summary>
    public void ShowPopup(List<AssistantInstance> runawayList)
    {
        Clear();

        foreach (var assistant in runawayList)
        {
            var go = Instantiate(slotPrefab, contentRoot);
            var slot = go.GetComponent<MineAssistantSlotUI>();
            if (slot != null)
            {
                slot.SetTempAssistant(assistant, null);
                slot.SetBlocked(true);
            }
            spawnedIcons.Add(go);
        }

        popupRoot.SetActive(true);
    }

    private void OnClickConfirm()
    {
        Clear();
        popupRoot.SetActive(false);
    }

    private void Clear()
    {
        foreach (var go in spawnedIcons)
        {
            Destroy(go);
        }
        spawnedIcons.Clear();
    }
}
