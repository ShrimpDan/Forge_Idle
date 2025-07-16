using UnityEngine;

public class ForgeVisualHandler : MonoBehaviour
{
    [Header("Assitant Roots")]
    [SerializeField] AssistantPrefabSO assistantPrefabSO;
    [SerializeField] Transform craftingSpawnRoot;
    [SerializeField] Transform sellingSpawnRoot;
    
    public void SpawnAssistantPrefab(AssistantInstance assi)
    {
        Transform spawnRoot = null;

        switch (assi.Specialization)
        {
            case SpecializationType.Crafting:
                spawnRoot = craftingSpawnRoot;
                break;

            case SpecializationType.Selling:
                spawnRoot = sellingSpawnRoot;
                break;
        }

        ClearSpawnRoot(assi.Specialization);
        Instantiate(assistantPrefabSO.GetAssistantByKey(assi.Key), spawnRoot);
    }

    public void ClearSpawnRoot(SpecializationType type)
    {
        Transform spawnRoot = null;

        switch (type)
        {
            case SpecializationType.Crafting:
                spawnRoot = craftingSpawnRoot;
                break;

            case SpecializationType.Selling:
                spawnRoot = sellingSpawnRoot;
                break;
        }

        if (spawnRoot != null && spawnRoot.childCount > 0)
        {
            foreach (Transform child in spawnRoot)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void ClearAllSpawnRoot()
    {
        foreach (Transform child in craftingSpawnRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in sellingSpawnRoot)
        {
            Destroy(child.gameObject);
        }
    }
}
