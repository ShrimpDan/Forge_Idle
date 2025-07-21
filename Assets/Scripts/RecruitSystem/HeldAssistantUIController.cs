using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    /// <summary>
    /// 보류 제자 리스트를 UI에 표시
    /// </summary>
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

    /// <summary>
    /// 보류 제자 UI 닫기
    /// </summary>
    public void HideHeldAssistantList()
    {
        heldUI.SetActive(false);
    }
}
