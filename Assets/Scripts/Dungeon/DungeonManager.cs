using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private GameManager gameManager;

    private TestDungeonData dungeonData;

    private WeaponHandler weaponHandler;
    private MonsterHandler monsterHandler;
    private RewardHandler rewardHandler;

    void Start()
    {
        gameManager = GameManager.Instance;
        Init();
    }

    public void Init()
    {
        weaponHandler = GetComponent<WeaponHandler>();
        monsterHandler = GetComponent<MonsterHandler>();
        rewardHandler = GetComponent<RewardHandler>();

        weaponHandler.Init(gameManager.Inventory.GetEquippedWeapons(), monsterHandler);
        monsterHandler.Init(dungeonData);
        rewardHandler.Init();
    }
}
