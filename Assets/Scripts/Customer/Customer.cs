using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;



public enum CustomerState
{ 
    Idle =0,
    MovingToBuyZone =1,
    InQueue =2,
    WaitintTurn =3,
    Purchasing =4,
    Exiting =5
}


public abstract class Customer : MonoBehaviour
{


    private CustomerManager customerManager;

    public static readonly int maxCount = 5; //5명 이상은 존재 안할꺼다

    public CustomerType Type => data.type;
    public CustomerJob Job => data.job;

    private bool isCrafted;

    [Header("손님 움직이는 지점 설정")]
    [SerializeField] protected List<Vector2> movePoint = new();
    [SerializeField] protected BuyPoint buyPoint;
    [SerializeField] private float stayMaxTime = 3f;
    [SerializeField] private float stayMinTime= 1f;

    [Header("Customerinfo")]
    [SerializeField] CustomerData data;
    [SerializeField] protected Transform[] moveWayPoint;
    [SerializeField] protected int gold;
    protected CustomerState state;

    //애니메이션
    protected Animator animator;
    protected bool IsMoving = true;
    protected Rigidbody2D rigid2D;
    [SerializeField] SpriteResolver spriteResolver;
    [SerializeField] SpriteLibrary spriteLibrary;


    private Coroutine moveRoutine; //큐에서 사용


    private float timer;


    protected virtual void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteResolver = GetComponentInChildren<SpriteResolver>();
        spriteLibrary = GetComponentInChildren<SpriteLibrary>();



    }

    protected virtual void Start()
    {
        StartCoroutine(CustomerFlow());
    }

    protected virtual void Update()
    {
    }


    protected virtual void FixedUpdate()
    {
        animator.SetBool("IsMove", IsMoving);
    }

    public void Init(CustomerData customerData)
    {
        customerManager = CustomerManager.Instance;
        data = customerData;

        isCrafted = false;
    }
    private IEnumerator CustomerFlow()
    {

        yield return MoveRandomPlace();
        yield return MoveToBuyZone();
        yield return JoinQueue();
        yield return WaitMyTurn();
        yield return PerformPurChase();
        yield return MoveToExit();
    }

    private IEnumerator MoveRandomPlace()
    {
        state = CustomerState.Idle;
        Vector2 pos = Vector2.zero;
        if (movePoint.Count >= 0)
        {
            pos = movePoint[Random.Range(0, movePoint.Count)];

        }
        yield return MoveingWayPoint(pos);
        float waitTime = Random.Range(stayMinTime, stayMaxTime);
        yield return WaitForSecondsCache.Wait(waitTime);


    }





    private IEnumerator MoveToBuyZone()
    {

        state = CustomerState.MovingToBuyZone;

        Vector2 qPos = buyPoint.GetLastPosition();
        yield return MoveingWayPoint(qPos);
    }



    private IEnumerator JoinQueue()
    {
        state = CustomerState.InQueue;

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
        IsMoving = true;
        while (Vector2.Distance(transform.position, wayPoint) > 0.1f)
        {
            Vector2 dir = (wayPoint - (Vector2)transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime * data.moveSpeed);


            yield return new WaitForFixedUpdate();
        }
        rigid2D.velocity = Vector2.zero; // 안전
        IsMoving = false;
        Debug.Log("IsMoving False");
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

    public void ChangeSpriteLibrary(SpriteLibraryAsset asset)
    {
        if (spriteLibrary != null && asset != null)
        {
            spriteLibrary.spriteLibraryAsset = asset;
        }
        else
        {
            Debug.Log("스프라이트 라이브러리 문제 발생");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < movePoint.Count; i++)
        {
            Vector3 worldPos = new Vector3(movePoint[i].x, movePoint[i].y, 0);

           
            Gizmos.DrawSphere(worldPos, 0.2f);
        }
    }
}
