using UnityEngine;

public class MineAssistantFSM : MonoBehaviour
{
    private Animator anim;
    private int mineIdx;
    private GameObject mineRoot;
    private MineSceneManager sceneManager;
    private bool isInitialized = false;

    private float wanderTimer = 0f;
    private float wanderInterval = 2.0f;

    private enum State { Idle, Walk, Work }
    private State state = State.Idle;

    private Vector2 targetPos;
    private bool hasTarget = false;
    private float moveSpeed = 2.5f; // 이동 속도

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
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        switch (state)
        {
            case State.Idle:
                wanderTimer += Time.deltaTime;
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
                    transform.position = Vector2.MoveTowards(current, targetPos, moveSpeed * Time.deltaTime);

                    if (Vector2.Distance(current, targetPos) < 0.08f)
                    {
                        hasTarget = false;
                        SetState(State.Work);
                    }
                }
                else
                {
                    // 타겟 없으면 바로 일
                    SetState(State.Work);
                }
                break;

            case State.Work:
                wanderTimer += Time.deltaTime;
                if (wanderTimer > 1.5f)
                {
                    SetState(State.Idle);
                }
                break;
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
                break;
            case State.Walk:
                anim.Play("Block");
                break;
            case State.Work:
                anim.Play("Slash");
                break;
        }
    }

    private void SetRandomDestination()
    {
        hasTarget = false;

        if (mineRoot == null)
        {
            Debug.LogError("[FSM] mineRoot==null! Init이 올바로 안 됨. 생성/Init 시점 구조 점검!");
            return;
        }

        Transform floor = FindDeepChild(mineRoot.transform, "Floor");
        if (floor == null)
        {
            Debug.LogError($"[FSM] Floor 오브젝트를 찾지 못함! mineRoot={mineRoot.name} (전체 하위 탐색 실패)");
            return;
        }

        var col = floor.GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError($"[FSM] Floor={floor.name}에 Collider2D 없음!");
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



}
