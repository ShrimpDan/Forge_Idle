using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandler : MonoBehaviour
{
    private MonsterHandler monsterHandler;

    private EquippedWeaponSlot[] equippedSlots;
    private Queue<EquippedWeaponSlot> attackQueue = new Queue<EquippedWeaponSlot>();

    [SerializeField] private RectTransform projectileRoot;
    [SerializeField] private GameObject projectilePrefab;

    public void Init(List<ItemInstance> equippedWeapons, MonsterHandler monsterHandler)
    {
        this.monsterHandler = monsterHandler;

        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            var slot = equippedSlots[i];

            if (i < equippedWeapons.Count)
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

        if (attackQueue.Count > 0)
        {
            var slot = attackQueue.Peek();
            if (slot.IsReady)
            {
                attackQueue.Dequeue();
                Attack(slot);
                slot.StartCooldown();
                StartCoroutine(WaitAndRequeue(slot));
            }
        }
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

        // 시작 위치: 플레이어 위치
        Vector2 startPos = WorldToLocalUIPosition(projectileRoot.position, projectileRoot);
        projectileRT.anchoredPosition = startPos;

        // 도착 위치: 몬스터 위치
        Vector2 targetPos = WorldToLocalUIPosition(monster.transform.position, projectileRoot);

        float jumpPower = 100f;      // 포물선 높이
        float duration = 0.5f;

        // 포물선 이동 + 회전 + 끝나면 제거
        projectileRT
            .DOJumpAnchorPos(targetPos, jumpPower, 1, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(go));

        // 회전 연출 (360도 회전)
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
