using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssistantPrefabSO", menuName = "Assistant/New AssistantPrefabSO")]
public class AssistantPrefabSO : ScriptableObject
{
    [SerializeField] private List<GameObject> assistantPrefabs;
    private Dictionary<string, GameObject> assistantDict;

    public void Init()
    {
        assistantDict = new Dictionary<string, GameObject>();

        foreach (var assistant in assistantPrefabs)
        {
            assistantDict[assistant.name] = assistant;
        }
    }

    public GameObject GetAssistantByKey(string key)
    {
        if (assistantDict == null || assistantDict.Count == 0)
            Init();

        if (assistantDict.TryGetValue(key, out GameObject assistant))
        {
            return assistant;
        }

        Debug.LogError("해당하는 제자 프리팹이 존재하지않습니다.");
        return null;
    }
}