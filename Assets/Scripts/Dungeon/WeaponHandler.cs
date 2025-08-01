using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private MonsterHandler monsterHandler;

    [Header("To Get Weapon")]
    [SerializeField] private EquippedWeaponSlot[] equippedSlots;
    private Queue<EquippedWeaponSlot> attackQueue = new Queue<EquippedWeaponSlot>();

    [Header("Weapon Prefab")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform monsterPos;

    [Header("Attack Setting")]
    [SerializeField] float attackDelay;
    [SerializeField] float maxJumpPower;
    [SerializeField] float minJumpPower;
    [SerializeField] float maxRotation;
    [SerializeField] float minRotation;
    [SerializeField] float duration;

    [Header("Player")]
    [SerializeField] Animator animator;
    private int attackHash = Animator.StringToHash("BlackSmith_Attack");

    private bool isAttack = false;

    public void Init(DungeonManager dungeonManager, List<ItemInstance> equippedWeapons)
    {
        this.dungeonManager = dungeonManager;
        monsterHandler = dungeonManager.MonsterHandler;

        for (int i = 0; i < equippedSlots.Length; i++)
        {
            var slot = equippedSlots[i];

            if (equippedWeapons[i] != null)
            {
                slot.gameObject.SetActive(true);
                slot.Init(equippedWeapons[i]);
                attackQueue.Enqueue(slot);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        foreach (var slot in equippedSlots)
        {
            slot.Tick(deltaTime);
        }

        if (attackQueue.Count > 0 && !isAttack && dungeonManager.IsRunning)
        {
            StartCoroutine(AttackCoroutine());
            isAttack = true;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (attackQueue.Count > 0 && dungeonManager.IsRunning)
        {
            var slot = attackQueue.Peek();
            Monster monster = monsterHandler.GetCurrentMonster();
            if (slot.IsReady && monster != null)
            {
                attackQueue.Dequeue();
                Attack(slot, monster);
                slot.StartCooldown();
                StartCoroutine(WaitAndRequeue(slot));
            }

            yield return WaitForSecondsCache.Wait(attackDelay);
        }

        isAttack = false;
    }

    private void Attack(EquippedWeaponSlot slot, Monster monster)
    {
        animator.Play(attackHash);
        SoundManager.Instance.Play("SFX_BattleThrow");
        SpawnProjectile(slot, monster);
    }

    // 투사체 소환 및 효과 적용
    private void SpawnProjectile(EquippedWeaponSlot slot, Monster monster = null)
    {
        Vector2 startPos = weaponPivot.position;
        Vector2 endPos = monsterPos.position;

        GameObject go = Instantiate(projectilePrefab, startPos, Quaternion.identity);

        if (go.TryGetComponent(out SpriteRenderer icon))
        {
            icon.sprite = IconLoader.GetIconByKey(slot.WeaponData.ItemKey);
        }

        go.transform.DOJump(endPos, Random.Range(minJumpPower, maxJumpPower), 1, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DamageToMonster(slot.WeaponData.GetTotalAttack(), monster);
                Destroy(go);
            });

        go.transform.DORotate(new Vector3(0, 0, Random.Range(minRotation, maxRotation)), duration, RotateMode.FastBeyond360);
    }

    private void DamageToMonster(float damage, Monster monster)
    {
        if (monster == null) return;

        monster.TakeDamage(damage);
    }

    // 쿨타임이 끝나면 무기 다시 공격 큐로
    private IEnumerator WaitAndRequeue(EquippedWeaponSlot slot)
    {
        yield return new WaitUntil(() => slot.IsReady);
        attackQueue.Enqueue(slot);
    }
}
