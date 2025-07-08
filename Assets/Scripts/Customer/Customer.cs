using System.Collections;
using UnityEngine;




public enum CustomerState
{ 
    Idle =0,
    MovingToBuyZone =1,
    InQueue =2,
    WaitintTurn =3,
    Purchasing =4,
    Exiting =5
}

//enum추가 하는것도 괜찮다. 
//will be SO

public abstract class Customer : MonoBehaviour
{
    private CustomerManager customerManager;

    public static readonly int maxCount = 5; //5명 이상은 존재 안할꺼다

    public CustomerType Type => data.type;
    public CustomerJob Job => data.job;

    public int Gold => gold;
    private bool isCrafted;
    [SerializeField] protected BuyPoint buyPoint;



    [Header("Customerinfo")]
    [SerializeField] CustomerData data;
    [SerializeField] protected Transform[] moveWayPoint;
    [SerializeField] protected int gold;







    protected CustomerState state;
    //애니메이션
    protected Animator animator;
    protected bool hasWeapon = false;

    private Transform targetPos;
    protected Rigidbody2D rigid2D; //buyingLine 

    private Coroutine moveRoutine;

    protected virtual void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        StartCoroutine(CustomerFlow());
    }

    protected virtual void Update()
    {

    }

    public void Init(CustomerData customerData)
    {
        customerManager = CustomerManager.Instance;
        data = customerData;

        isCrafted = false;
    }


    private IEnumerator CustomerFlow()
    {

        yield return MoveToBuyZone();
        yield return JoinQueue();
        yield return WaitMyTurn();
        yield return PerformPurChase();
        yield return MoveToExit();
    }


    private IEnumerator MoveToBuyZone()
    {
        
        state = CustomerState.MovingToBuyZone;
        SetAnimationState(state);
        Vector2 qPos = buyPoint.GetLastPosition();
        yield return MoveingWayPoint(qPos);
    }



    private IEnumerator JoinQueue()
    {
        state = CustomerState.InQueue;
        SetAnimationState(state);
        buyPoint.CustomerIn(this);
        
        customerManager.CustomerEvent?.RaiseCustomerArrived(this); //이벤트 연결
        yield return null;

    }


    private IEnumerator WaitMyTurn()
    {
        state = CustomerState.WaitintTurn;
        yield return new WaitUntil(() => buyPoint.IsCustomFirst(this));
        
        yield return new WaitUntil(() => isCrafted);
    }

    protected virtual IEnumerator PerformPurChase()
    {
        state = CustomerState.Purchasing;
        Interact();
        buyPoint.CustomerOut();
        yield return null;

    }

    private IEnumerator MoveToExit()
    {
        state = CustomerState.Exiting;
        yield return MoveingWayPoint(moveWayPoint[1].position); //이거 고정시키는거 좋은 방법 없을까

        CustomerExit();
    }


    protected IEnumerator MoveingWayPoint(Vector2 wayPoint)
    {
        animator?.SetBool("HasWeapon", hasWeapon);
        while (Vector2.Distance(transform.position, wayPoint) > 0.1f)
        {
            Vector2 dir = (wayPoint - (Vector2)transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime * data.moveSpeed);

            yield return new WaitForFixedUpdate();
        }
    }


    public void SetQueuePos(Vector2 queuePos)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(MoveingWayPoint(queuePos));//이동 

    }

    protected virtual void CustomerExit() //큐에서 나가는 메서드
    {
        CustomerManager.Instance.CustomerExit(this);
        Destroy(gameObject);
    }
    public abstract void Interact();
    public void NotifiedCraftWeapon()
    {
        isCrafted = true;
    }

    protected void SetAnimationState(CustomerState state)
    {
        animator?.SetInteger("State", (int)state);
        animator?.SetBool("HasWeapon", hasWeapon);
        if (state == CustomerState.MovingToBuyZone || state == CustomerState.Exiting)
        {
            animator.speed = data.moveSpeed / 2.0f;
        }
        else
        {
            animator.speed = 1.0f;
        }

    }

}
