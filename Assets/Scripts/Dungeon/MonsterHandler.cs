using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MonsterHandler : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private DungeonUI dungeonUI;

    private DungeonData dungeonData;


    [Header("SpawnMosnter Setting")]
    [SerializeField] MonsterPrefabSO monsterPrefabSO;
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private Transform monsterPos;

    private Queue<Monster> monstersQueue = new();
    private Monster currentMonster;

    private int killedMonster;

    public System.Action OnDungeonCleared;

    public void Init(DungeonManager dungeonManager)
    {
        this.dungeonManager = dungeonManager;
        dungeonUI = dungeonManager.DungeonUI;
        dungeonData = dungeonManager.DungeonData;

        InitMonserPool();
        killedMonster = 0;

        SpawnNextMonster();
    }

    private void InitMonserPool()
    {
        // 일반 몬스터 생성
        for (int i = 0; i < dungeonData.MaxMonster; i++)
        {
            string key = dungeonData.NormalMonsterKeys[Random.Range(0, dungeonData.NormalMonsterKeys.Count)];

            GameObject go = Instantiate(monsterPrefabSO.GetMonsterByKey(key), spawnRoot);
            Monster monster = go.GetComponent<Monster>();

            // 몬스터 초기 설정
            monster.Init(this, dungeonData.MonsterHp);
            monster.OnDeath += HandleMonsterDeath;
            go.SetActive(false);

            // 몬스터 큐에 추가
            monstersQueue.Enqueue(monster);
        }

        // 보스 몬스터 생성
        GameObject bossGo = Instantiate(monsterPrefabSO.GetMonsterByKey(dungeonData.BossMonsterKey), spawnRoot);
        Monster boss = bossGo.GetComponent<Monster>();

        // 보스 몬스터 초기 설정
        boss.Init(this, dungeonData.BossHp, isBoss: true);
        boss.OnDeath += HandleMonsterDeath;
        bossGo.SetActive(false);

        // 몬스터 큐에 추가
        monstersQueue.Enqueue(boss);
    }

    private void SpawnNextMonster()
    {
        if (monstersQueue.Count == 0)
        {
            Debug.Log("던전 클리어!");
            OnDungeonCleared?.Invoke();
            return;
        }

        Monster monster = monstersQueue.Dequeue();
        monster.gameObject.SetActive(true);
        monster.transform.DOMoveX(monsterPos.position.x, 0.3f).SetEase(Ease.OutBack).OnComplete(() => currentMonster = monster);
    }

    private void HandleMonsterDeath()
    {
        currentMonster = null;

        killedMonster++;
        dungeonUI.UpdateMonsterUI(killedMonster, dungeonData.MaxMonster);

        dungeonManager.AddReward(Random.Range(dungeonData.MinCount, dungeonData.MaxCount));

        if (monstersQueue.Count != 0)
            SpawnNextMonster();
        else
            dungeonManager.DungeonClear(true);
    }

    public Monster GetCurrentMonster()
    {
        return currentMonster;
    }

    public void ClearCurrentMonster()
    {
        currentMonster = null;
    }
}
