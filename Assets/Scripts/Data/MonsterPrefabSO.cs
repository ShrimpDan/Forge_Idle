using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterPrefabSO", menuName = "Monster/New MonsterPrefabSO")]
public class MonsterPrefabSO : ScriptableObject
{
    [SerializeField] private List<GameObject> monsterPrefabs;
    private Dictionary<string, GameObject> monsterDict;

    public void Init()
    {
        monsterDict = new Dictionary<string, GameObject>();

        foreach (var monster in monsterPrefabs)
        {
            monsterDict[monster.name] = monster;
        }
    }

    public GameObject GetMonsterByKey(string key)
    {
        if (monsterDict == null || monsterDict.Count == 0)
            Init();

        if (monsterDict.TryGetValue(key, out GameObject monster))
        {
            return monster;
        }

        Debug.LogError("해당하는 몬스터 프리팹이 존재하지않습니다.");
        return null;
    }
}
