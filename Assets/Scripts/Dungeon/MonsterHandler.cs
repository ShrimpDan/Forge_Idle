using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHandler : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private DungeonUI dungeonUI;
    
    private TestDungeonData dungeonData;

    [SerializeField] private RectTransform spawnRoot;
    [SerializeField] private GameObject normalMonsterPrefab;
    [SerializeField] private GameObject bossMonsterPrefab;

    private Queue<Monster> monstersQueue = new();
    private Monster currentMonster;

    private int killedMonster;
    private int maxMonster = 20;

    public System.Action OnDungeonCleared;

    public void Init(DungeonManager dungeonManager)
    {
        this.dungeonManager = dungeonManager;
        dungeonUI = dungeonManager.DungeonUI;
        dungeonData = dungeonManager.DungeonData;

        // 일반 몬스터 생성
        for (int i = 0; i < maxMonster; i++)
        {
            var go = Instantiate(normalMonsterPrefab, spawnRoot);
            var monster = go.GetComponent<Monster>();
            monster.Init(dungeonData.MonsterHp);
            monster.OnDeath += HandleMonsterDeath;
            monstersQueue.Enqueue(monster);
            go.SetActive(false);
        }

        // 보스 몬스터 생성
        var bossGo = Instantiate(bossMonsterPrefab, spawnRoot);
        var boss = bossGo.GetComponent<Monster>();
        boss.Init(dungeonData.BossHp, isBoss: true);
        boss.OnDeath += HandleMonsterDeath;
        bossGo.SetActive(false);
        monstersQueue.Enqueue(boss);

        killedMonster = 0;
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
        currentMonster = null;

        killedMonster++;
        dungeonUI.UpdateMonsterUI(killedMonster, maxMonster);

        dungeonManager.AddReward(Random.Range(3, 10));

        if (monstersQueue.Count != 0)
            SpawnNextMonster();
        else
            dungeonManager.DungeonClear(true);
    }

    public Monster GetCurrentMonster()
    {
        return currentMonster;
    }
}
