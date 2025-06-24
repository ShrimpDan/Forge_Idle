using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistantTab : MonoBehaviour
{
    [Header("To Create Assistant Slot")]
    [SerializeField] GameObject assiPrefab;
    [SerializeField] Transform assiRoot;

    [Header("Selected Assistant")]
    [SerializeField] Image craftAssi;
    [SerializeField] Image sellAssi;
    [SerializeField] Image upgradeAssi;

    [Header("To Create Bonus Stat")]
    [SerializeField] GameObject bonusStatPrefab;
    [SerializeField] Transform craftRoot;
    [SerializeField] Transform sellRoot;
    [SerializeField] Transform upgradeRoot;

    private List<AssistantSlot> assistantSlots;

    public void AddAssistant(TestAssistantData data)
    {
        if (data == null) return;

        GameObject obj = Instantiate(assiPrefab, assiRoot);

        if (obj.TryGetComponent(out AssistantSlot assiSlot))
        {
            assiSlot.Init(data);
            assistantSlots.Add(assiSlot);
        }
    }
}
