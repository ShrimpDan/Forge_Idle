using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandler : MonoBehaviour
{
    private MonsterHandler monsterHandler;

    [SerializeField] private EquippedWeaponSlot[] equippedSlots;
    private Queue<EquippedWeaponSlot> attackQueue = new Queue<EquippedWeaponSlot>();

    [Header("Weapon Prefab")]
    [SerializeField] private RectTransform projectileRoot;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Attack Setting")]
    [SerializeField] float attackDelay;
    private bool isAttack = false;

    public void Init(List<ItemInstance> equippedWeapons, MonsterHandler monsterHandler)
    {
        this.monsterHandler = monsterHandler;

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

        if (attackQueue.Count > 0 && !isAttack)
        {
            StartCoroutine(AttackCoroutine());
            isAttack = true;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (attackQueue.Count > 0)
        {
            var slot = attackQueue.Peek();
            if (slot.IsReady)
            {
                attackQueue.Dequeue();
                Attack(slot);
                slot.StartCooldown();
                StartCoroutine(WaitAndRequeue(slot));
            }

            yield return new WaitForSeconds(attackDelay);
        }

        isAttack = false;
    }

    private void Attack(EquippedWeaponSlot slot)
    {
        // 무기 던짐
        Monster monster = monsterHandler.GetCurrentMonster();
        if (monster == null) return;

        SpawnProjectile(slot, monster);
    }

    private void SpawnProjectile(EquippedWeaponSlot slot, Monster monster)
    {
        GameObject go = Instantiate(projectilePrefab, projectileRoot);
        Image img = go.GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>(slot.WeaponData.Data.IconPath);

        RectTransform projectileRT = go.GetComponent<RectTransform>();

        Vector2 startPos = WorldToLocalUIPosition(projectileRoot.position, projectileRoot);
        projectileRT.anchoredPosition = startPos;

        Vector2 targetPos = WorldToLocalUIPosition(monster.transform.position, projectileRoot);

        float jumpPower = 100f;
        float duration = 0.5f;

        projectileRT
            .DOJumpAnchorPos(targetPos, jumpPower, 1, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(go));

        projectileRT
            .DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }

    // 쿨타임이 끝나면 무기 다시 공격 큐로
    private IEnumerator WaitAndRequeue(EquippedWeaponSlot slot)
    {
        yield return new WaitUntil(() => slot.IsReady);
        attackQueue.Enqueue(slot);
    }

    private Vector2 WorldToLocalUIPosition(Vector3 worldPos, RectTransform canvasRoot)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRoot, screenPos, null, out Vector2 localPoint);
        return localPoint;
    }
}
