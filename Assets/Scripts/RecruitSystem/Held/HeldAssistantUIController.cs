using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// HeldAssistantUIController.cs
// 보류 중인 제자 리스트를 UI 상에 표시하거나 숨기는 기능을 담당합니다.
// 각 제자 종이를 지정된 좌표에 생성하여 배치하며, 해당 UI의 활성/비활성 처리를 포함합니다.

public class HeldAssistantUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject heldUI;
    [SerializeField] private Transform heldRoot;
    [SerializeField] private GameObject heldAssistantPrefab;
    [SerializeField] private HeldListButtonHandler listButtonHandler;

    [Header("좌표 지정 (최대 5개)")]
    private readonly Vector2[] paperPositions = new Vector2[]
    {
        new Vector2(190, -347.15f),
        new Vector2(570, -347.15f),
        new Vector2(950, -347.15f),
        new Vector2(370, -750f),
        new Vector2(750, -750f)
    };

    private List<GameObject> activeHeldItems = new();

    // 보류 제자 리스트를 UI에 표시
    public void ShowHeldAssistantList(List<AssistantInstance> heldList)
    {
        heldUI.SetActive(true);

        foreach (var item in activeHeldItems)
        {
            if (item != null)
                Destroy(item);
        }
        activeHeldItems.Clear();

        int count = Mathf.Min(heldList.Count, paperPositions.Length);

        for (int i = 0; i < count; i++)
        {
            AssistantInstance instance = heldList[i];

            GameObject paper = Instantiate(heldAssistantPrefab, heldRoot);
            paper.SetActive(true);

            RectTransform rt = paper.GetComponent<RectTransform>();
            rt.anchoredPosition = paperPositions[i];

            HeldAssistantItem item = paper.GetComponent<HeldAssistantItem>();
            item.Init(instance, listButtonHandler);

            activeHeldItems.Add(paper);
        }
    }

    // 보류 제자 UI 닫기
    public void HideHeldAssistantList()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        heldUI.SetActive(false);
    }
}
