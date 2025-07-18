using UnityEngine;
using UnityEngine.AI;

public class MineAssistantFSM : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    private int mineIdx;
    private GameObject mineRoot;
    private MineSceneManager sceneManager;

    private float wanderTimer = 0f;
    private float wanderInterval = 2.0f;
    private enum State { Idle, Walk, Work }
    private State state = State.Idle;

    public void Init(AssistantInstance assistant, int mineIdx, GameObject mineRoot, MineSceneManager mgr)
    {
        this.mineIdx = mineIdx;
        this.mineRoot = mineRoot;
        this.sceneManager = mgr;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        SetState(State.Idle);
        wanderTimer = 0;
    }

    private void Update()
    {
        if (agent == null) return;

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
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
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
        var floor = mineRoot.transform.Find("Floor");
        if (floor != null)
        {
            var bounds = floor.GetComponent<Collider2D>().bounds;
            Vector2 dest = Vector2.zero;
            for (int i = 0; i < 20; ++i)
            {
                dest = new Vector2(
                    UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                    UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
                );
                if (!Physics2D.OverlapCircle(dest, 0.3f, LayerMask.GetMask("Obstacle")))
                {
                    agent.SetDestination(dest);
                    break;
                }
            }
        }
    }
}
