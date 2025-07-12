using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private float stayMaxTime = 3f;
    [SerializeField] private float stayMinTime = 1f;
    protected BuyPoint buyPoint;

    [Header("Customerinfo")]
    [SerializeField] CustomerData data;
    [SerializeField] protected Transform[] moveWayPoint;
    [SerializeField] protected int gold;

    [SerializeField] private float waitAngryTime = 10f;
    protected CustomerState state;

    //애니메이션
    protected Animator animator;
    protected bool IsMoving = true;
    protected Rigidbody2D rigid2D;
    //말풍선
    protected CustomerSpeechBubble speech;

    [SerializeField] SpriteLibrary spriteLibrary;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("PurchaseEffect")]
    [SerializeField] TextMeshPro goldText;

    

    private Coroutine moveRoutine; //큐에서 사용
    private Coroutine customerFlowCoroutine;

    private Coroutine StopTime;
    public bool IsAngry { get; private set; }


    private float timer;

    //풀링
    private GameObject sourcePrefab;

    public void Init(CustomerData _customerData, BuyPoint _buyPoint)
    {
        customerManager = CustomerManager.Instance;
        data = _customerData;
        buyPoint = _buyPoint;
        isCrafted = false;
        IsAngry = false;
    }


    protected virtual void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
       
        spriteLibrary = GetComponentInChildren<SpriteLibrary>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        speech = GetComponentInChildren<CustomerSpeechBubble>();

    }

    protected virtual void Start()
    {
       
    }
    protected virtual void OnEnable()
    {       
        if (customerFlowCoroutine != null)
        {
            StopCoroutine(customerFlowCoroutine);
        }
        customerFlowCoroutine = StartCoroutine(CustomerFlow());
      
    }


    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        customerFlowCoroutine = null;
        moveRoutine = null;
        speech.Hide();
        timer = 0;

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
    public void SetSourcePrefab(GameObject prefab)
    {
        this.sourcePrefab = prefab; 
    }

    protected virtual IEnumerator CustomerFlow()
    {

        yield return MoveRandomPlace();
        yield return MoveToBuyZone();
        yield return JoinQueue();
        yield return WaitMyTurn();
        yield return PerformPurChase();
        

    }

    protected IEnumerator MoveRandomPlace()
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





    protected IEnumerator MoveToBuyZone()
    {

        state = CustomerState.MovingToBuyZone;

        Vector2 qPos = buyPoint.GetLastPosition();
        yield return MoveingWayPoint(qPos);
    }



    protected IEnumerator JoinQueue()
    {
        state = CustomerState.InQueue;

        buyPoint.CustomerIn(this);
        
        customerManager.CustomerEvent?.RaiseCustomerArrived(this); //이벤트 연결
        yield return null;

    }

    protected IEnumerator AngryTime()
    {
        while (timer < 10)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        speech.Show("Angry");
        IsAngry = true;
        buyPoint.CustomerOut();
        yield return MoveToExit();
    }



    protected IEnumerator WaitMyTurn()
    {
        state = CustomerState.WaitintTurn;
        if (RandomChance() == true)
        { 
        speech.Show("ThinkAnimation");
        }
        if (StopTime != null)
        {
            StopCoroutine(StopTime);
            StopTime = null;
        }
        StopTime = StartCoroutine(AngryTime());

        yield return new WaitUntil(() => buyPoint.IsCustomFirst(this));
        yield return new WaitUntil(() => isCrafted);
        speech.Show("Idle");
    }

    protected virtual IEnumerator PerformPurChase()
    {
        state = CustomerState.Purchasing;
        
        speech.Show("Happy");
        Interact();
        yield return WaitForSecondsCache.Wait(1f);
        speech.Hide();
        buyPoint.CustomerOut();
        yield return MoveToExit();

    }

    protected IEnumerator MoveToExit()
    {
        state = CustomerState.Exiting;

        if (moveWayPoint != null && moveWayPoint.Length > 1 && moveWayPoint[1] != null)
        {
            yield return MoveingWayPoint(moveWayPoint[1].position);
        }
        else
        {
            Debug.LogWarning($"[Customer] 퇴장 경로(moveWayPoint)가 제대로 설정되지 않았습니다. 오브젝트: {this.gameObject.name}", this.gameObject);
        }

        CustomerExit();
    }


    protected IEnumerator MoveingWayPoint(Vector2 wayPoint)
    {
        IsMoving = true;
        while (Vector2.Distance(transform.position, wayPoint) > 0.1f)
        {
            Vector2 dir = (wayPoint - (Vector2)transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime * data.moveSpeed);
            if (dir.x <= 0)
            {
                spriteRenderer.flipX = true; // ← 왼쪽이면 flip

            }
            else
            {
                spriteRenderer.flipX = false; // → 오른쪽이면 flip
            }   
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
        Debug.Log("손님나감 호출");
        CustomerManager.Instance.CustomerExit(this);
        PoolManager.Instance.Return(this.gameObject, this.sourcePrefab);
    }
    public abstract void Interact();
    public void NotifiedCraftWeapon()
    {
        isCrafted = true;
        if (StopTime != null)
        {
            StopCoroutine(StopTime);
            StopTime = null;
        }
        
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
    private void OnDrawGizmosSelected() // 움직이는 포인트 시각화
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < movePoint.Count; i++)
        {
            Vector3 worldPos = new Vector3(movePoint[i].x, movePoint[i].y, 0);


            Gizmos.DrawSphere(worldPos, 0.2f);
        }
    }

    private bool RandomChance() //랜덤으로 값 뽑아낼때 사용
    {
        int dice = Random.Range(0,10);
        if (dice > 5)
        {
            return true;
        }

        return false;

    }
  
}
