using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHandler : MonoBehaviour
{
    private TestDungeonData dungeonData;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject normalMonsterPrefab;
    [SerializeField] private GameObject bossMonsterPrefab;

    private Queue<Monster> monstersQueue = new();
    private Monster currentMonster;

    public System.Action OnDungeonCleared;

    public void Init(TestDungeonData data)
    {
        if (data == null) return;
        
        dungeonData = data;

        for (int i = 0; i < 40; i++)
        {
            var go = Instantiate(normalMonsterPrefab, spawnPoint.position, Quaternion.identity, transform);
            var monster = go.GetComponent<Monster>();
            monster.Init(dungeonData.MonsterHp);
            monster.OnDeath += HandleMonsterDeath;
            monstersQueue.Enqueue(monster);
            go.SetActive(false);


        }

        var bossGo = Instantiate(bossMonsterPrefab, spawnPoint.position, Quaternion.identity, transform);
        var boss = bossGo.GetComponent<Monster>();
        boss.Init(dungeonData.BossHp, isBoss: true);
        boss.OnDeath += HandleMonsterDeath;
        bossGo.SetActive(false);
        monstersQueue.Enqueue(boss);

        SpawnNextMonster();
    }

    private void SpawnNextMonster()
    {
        if (monstersQueue.Count == 0)
        {
            Debug.Log("던전 클리어!");
            OnDungeonCleared?.Invoke();
            return;
        }

        currentMonster = monstersQueue.Dequeue();
        currentMonster.gameObject.SetActive(true);
    }

    private void HandleMonsterDeath()
    {
        // RewardHandler에 보상 추가
    }

    public Monster GetCurrentMonster()
    {
        return currentMonster;
    }
}
