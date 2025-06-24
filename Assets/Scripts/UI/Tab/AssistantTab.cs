using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistantTab : MonoBehaviour
{
    private AssistantManager assistant;

    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("To Create Assistant")]
    [SerializeField] GameObject assiSlotPrefab;
    [SerializeField] Transform craftSlotRoot;
    [SerializeField] Transform gemSlotRoot;
    [SerializeField] Transform upgradeSlotRoot;

    [Header("Selected Assistant")]
    [SerializeField] Image craftAssi;
    [SerializeField] Image sellAssi;
    [SerializeField] Image upgradeAssi;

    [Header("To Create Bonus Stat")]
    [SerializeField] GameObject bonusStatPrefab;
    [SerializeField] Transform craftStatRoot;
    [SerializeField] Transform sellStatRoot;
    [SerializeField] Transform upgradeStatRoot;

    private Queue<GameObject> pooledSlots = new Queue<GameObject>();
    private List<GameObject> activeSlots = new List<GameObject>();

    public void Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        SwitchTab(0);
    }

    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;

            tabPanels[i].SetActive(isSelected);

            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }
    }

    private GameObject GetSlotFromPool()
    {
        if (pooledSlots.Count > 0)
            return pooledSlots.Dequeue();

        return Instantiate(assiSlotPrefab);
    }
}
