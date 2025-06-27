using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandler : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private MonsterHandler monsterHandler;

    [SerializeField] private EquippedWeaponSlot[] equippedSlots;
    private Queue<EquippedWeaponSlot> attackQueue = new Queue<EquippedWeaponSlot>();

    [Header("Weapon Prefab")]
    [SerializeField] private RectTransform BattleSlot;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private RectTransform playerAnchor;
    [SerializeField] private RectTransform monsterAnchor;

    [Header("Attack Setting")]
    [SerializeField] float attackDelay;
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
            yield return WaitForSecondsCache.Wait(attackDelay);

            var slot = attackQueue.Peek();
            if (slot.IsReady)
            {
                attackQueue.Dequeue();
                Attack(slot);
                slot.StartCooldown();
                StartCoroutine(WaitAndRequeue(slot));
            }
        }

        isAttack = false;
    }

    private void Attack(EquippedWeaponSlot slot)
    {
        // 무기 던짐
        Monster monster = monsterHandler.GetCurrentMonster();

        if (monster != null)
        {
            SpawnProjectile(slot, monster);
        }
    }

    private void SpawnProjectile(EquippedWeaponSlot slot, Monster monster = null)
    {
        Vector2 startPos = GetLocalPosFromAnchor(playerAnchor);
        Vector2 endPos = GetLocalPosFromAnchor(monsterAnchor);

        GameObject go = Instantiate(projectilePrefab, BattleSlot);

        if (go.TryGetComponent(out Image icon))
        {
            icon.sprite = IconLoader.GetIcon(slot.WeaponData.Data.IconPath);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = startPos;

        rt.DOJumpAnchorPos(endPos, 200f, 1, 1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DamageToMonster(slot.WeaponData.GetTotalAttack(), monster);
                Destroy(go);
            });

        rt.DORotate(new Vector3(0, 0, 540f), 1f, RotateMode.FastBeyond360);
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

    private Vector2 GetLocalPosFromAnchor(RectTransform anchor)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, anchor.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            BattleSlot, screenPos, Camera.main, out Vector2 localPoint);
        return localPoint;
    }
}
