using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MineAssistantFSM : MonoBehaviour
{
    private Animator anim;
    private int mineIdx;
    private GameObject mineRoot;
    private MineSceneManager sceneManager;
    private bool isInitialized = false;

    private float wanderTimer = 0f;
    private float wanderInterval = 1.0f;

    private enum State { Idle, Walk, Work }
    private State state = State.Idle;

    private Vector2 targetPos;
    private bool hasTarget = false;

    // ---- FSM 및 버프 관련 ----
    [Header("FSM 속도/버프 설정")]
    public float baseMoveSpeed = 2.5f;
    public float moveSpeed = 2.5f;

    private float fsmSpeedMultiplier = 1f;

    // --- 버프/쿨타임 데이터 ---
    private bool isBuffActive = false;
    private bool isCooldown = false;
    private float buffDuration = 3600f; // 1시간(초)
    private float buffTimer = 0f;

    private float cooldownDuration = 21600f; // 6시간(초)
    private float cooldownTimer = 0f;

    private float resourceBuffMultiplier = 1.0f; // 생산량 증가(1.1f)
    private TextMeshPro cooldownText;
    private GameObject cooldownTextObj;

    // 어시스턴트 정보 (버프-자원 연동용)
    private AssistantInstance assistantInstance;

    private List<UnityEngine.Tilemaps.Tilemap> obstacleTilemaps;

    // 작업 애니메이션 카운트
    private int workAnimCount = 0;
    private int workAnimMax = 0;

    // ---- 초기화 ----
    public void Init(AssistantInstance assistant, int mineIdx, GameObject mineRoot, MineSceneManager mgr)
    {
        this.mineIdx = mineIdx;
        this.mineRoot = mineRoot;
        this.sceneManager = mgr;
        anim = GetComponent<Animator>();
        SetState(State.Idle);
        wanderTimer = 0;
        hasTarget = false;
        isInitialized = true;
        assistantInstance = assistant;

        var manager = mgr.mineGroups[mineIdx];
        obstacleTilemaps = manager.obstacleTilemaps;
        CreateCooldownText();
        UpdateAnimatorSpeed();
    }

    private void CreateCooldownText()
    {
        if (cooldownTextObj != null)
        {
            Destroy(cooldownTextObj);
        }
        cooldownTextObj = new GameObject("CooldownText");
        cooldownTextObj.transform.SetParent(transform);
        cooldownTextObj.transform.localPosition = new Vector3(0, 2.0f, 0);
        cooldownText = cooldownTextObj.AddComponent<TextMeshPro>();
        cooldownText.fontSize = 3;
        cooldownText.alignment = TextAlignmentOptions.Center;
        cooldownText.color = Color.yellow;
        cooldownText.text = "";
    }


    private void DestroyCooldownText()
    {
        if (cooldownTextObj != null)
        {
            Destroy(cooldownTextObj);
            cooldownTextObj = null;
            cooldownText = null;
        }
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        // 버프/쿨타임 처리
        HandleBuffAndCooldown();

        switch (state)
        {
            case State.Idle:
                wanderTimer += Time.deltaTime * fsmSpeedMultiplier;
                if (wanderTimer > wanderInterval)
                {
                    SetRandomDestination();
                    SetState(State.Walk);
                }
                break;

            case State.Walk:
                if (hasTarget)
                {
                    Vector2 current = transform.position;
                    if (IsObstacleBetween(current, targetPos))
                    {
                        hasTarget = false;
                        SetState(State.Idle);
                        break;
                    }
                    transform.position = Vector2.MoveTowards(current, targetPos, moveSpeed * fsmSpeedMultiplier * Time.deltaTime);
                    if (Vector2.Distance(current, targetPos) < 0.08f)
                    {
                        hasTarget = false;
                        SetState(State.Work);
                    }
                }
                else
                {
                    SetState(State.Work);
                }
                break;

            case State.Work:
                wanderTimer += Time.deltaTime * fsmSpeedMultiplier;
                if (wanderTimer > 1.0f)
                {
                    wanderTimer = 0;
                    workAnimCount++;
                    if (workAnimCount < workAnimMax)
                    {
                        anim.Play("Slash", 0, 0);
                    }
                    else
                    {
                        SetState(State.Idle);
                    }
                }
                break;
        }
    }

    private void HandleBuffAndCooldown()
    {
        // 텍스트 상태 갱신
        if (isBuffActive)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer > 0)
            {
                UpdateCooldownText(buffTimer);
            }
            else
            {
                EndBuff();
            }
        }
        else if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0)
            {
                UpdateCooldownText(cooldownTimer);
            }
            else
            {
                isCooldown = false;
                if (cooldownText != null) cooldownText.text = "";
                DestroyCooldownText();
            }
        }
        else
        {
            if (cooldownText != null) cooldownText.text = "";
            DestroyCooldownText();
        }
    }

    private void UpdateCooldownText(float t)
    {
        if (cooldownText == null) return;
        TimeSpan ts = TimeSpan.FromSeconds(Mathf.Max(0, t));
        cooldownText.text = $"{ts:hh\\:mm\\:ss}";
    }

    private void StartBuff()
    {
        if (cooldownText == null) CreateCooldownText();
        isBuffActive = true;
        buffTimer = buffDuration;
        isCooldown = false;
        fsmSpeedMultiplier = 2f;
        resourceBuffMultiplier = 1.1f;
        moveSpeed = baseMoveSpeed * 2f;
        UpdateAnimatorSpeed();
    }

    private void EndBuff()
    {
        isBuffActive = false;
        fsmSpeedMultiplier = 1f;
        resourceBuffMultiplier = 1.0f;
        moveSpeed = baseMoveSpeed;
        UpdateAnimatorSpeed();
        isCooldown = true;
        cooldownTimer = cooldownDuration;
        if (cooldownText != null) cooldownText.text = "";
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked!");
        if (!isBuffActive && !isCooldown)
        {
            if (cooldownText == null) CreateCooldownText();
            StartBuff();
        }
    }


    private void SetState(State newState)
    {
        state = newState;
        wanderTimer = 0;
        if (anim == null) return;

        switch (state)
        {
            case State.Idle:
                anim.Play("Idle");
                wanderInterval = UnityEngine.Random.Range(0.7f, 1.2f);
                break;
            case State.Walk:
                anim.Play("Block");
                SoundManager.Instance?.Play("MineWalkSound");
                break;
            case State.Work:
                workAnimMax = UnityEngine.Random.Range(3, 11);
                workAnimCount = 0;
                anim.Play("Slash");
                SoundManager.Instance?.Play("MineSound");
                break;
        }
    }

    private void SetRandomDestination()
    {
        hasTarget = false;

        if (mineRoot == null)
        {
            return;
        }

        Transform floor = FindDeepChild(mineRoot.transform, "Floor");
        if (floor == null)
        {
            return;
        }

        var col = floor.GetComponent<Collider2D>();
        if (col == null)
        {
            return;
        }

        var bounds = col.bounds;
        for (int i = 0; i < 20; ++i)
        {
            var dest = new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
            );
            if (!Physics2D.OverlapCircle(dest, 0.3f, LayerMask.GetMask("Obstacle")))
            {
                targetPos = dest;
                hasTarget = true;
                return;
            }
        }

        for (int i = 0; i < 20; ++i)
        {
            var dest = new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
            );
            bool isBlocked = false;
            if (obstacleTilemaps != null)
            {
                foreach (var tmap in obstacleTilemaps)
                {
                    Vector3Int cell = tmap.WorldToCell(dest);
                    if (tmap.HasTile(cell))
                    {
                        isBlocked = true;
                        break;
                    }
                }
            }
            if (!isBlocked && !Physics2D.OverlapCircle(dest, 0.3f, LayerMask.GetMask("Obstacle")))
            {
                targetPos = dest;
                hasTarget = true;
                return;
            }
        }
        targetPos = transform.position;
        hasTarget = false;
    }

    public static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
    private bool IsObstacleBetween(Vector2 from, Vector2 to)
    {
        if (obstacleTilemaps == null) return false;
        float dist = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(dist / 0.1f);
        for (int i = 1; i <= steps; ++i)
        {
            Vector2 pos = Vector2.Lerp(from, to, i / (float)steps);
            foreach (var tmap in obstacleTilemaps)
            {
                Vector3Int cell = tmap.WorldToCell(pos);
                if (tmap.HasTile(cell))
                    return true;
            }
        }
        if (Physics2D.OverlapCircle(to, 0.3f, LayerMask.GetMask("Obstacle")))
            return true;

        return false;
    }

    // 버프/쿨타임 값 복원
    public void LoadBuffState(bool buffActive, float buffRemain, bool cooldown, float cooldownRemain)
    {
        isBuffActive = buffActive;
        isCooldown = cooldown;
        buffTimer = buffActive ? buffRemain : 0f;
        cooldownTimer = cooldown ? cooldownRemain : 0f;

        // 상태별 값 반영 (moveSpeed 등도!)
        if (isBuffActive)
        {
            fsmSpeedMultiplier = 2f;
            resourceBuffMultiplier = 1.1f;
            moveSpeed = baseMoveSpeed * 2f;
        }
        else
        {
            fsmSpeedMultiplier = 1f;
            resourceBuffMultiplier = 1.0f;
            moveSpeed = baseMoveSpeed;
        }
        UpdateAnimatorSpeed();

        // UI 텍스트 상태 동기화
        if (isBuffActive || isCooldown)
        {
            if (cooldownText == null) CreateCooldownText();
            UpdateCooldownText(isBuffActive ? buffTimer : cooldownTimer);
        }
        else
        {
            DestroyCooldownText();
        }
    }

    private void UpdateAnimatorSpeed()
    {
        if (anim != null)
            anim.speed = fsmSpeedMultiplier; // 버프: 2, 일반: 1
    }
    public float ResourceBuffMultiplier() => resourceBuffMultiplier;
    public bool IsBuffActive() => isBuffActive;
    public float GetBuffRemain() => isBuffActive ? buffTimer : 0f;
    public bool IsCooldown() => isCooldown;
    public float GetCooldownRemain() => isCooldown ? cooldownTimer : 0f;
}
