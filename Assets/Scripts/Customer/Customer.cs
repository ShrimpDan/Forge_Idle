using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;




public enum CustomerState
{ 
    Idle,
    MovingToBuyZone,
    InQueue,
    WaitintTurn,
    Purchasing,
    Exiting
}

//enum추가 하는것도 괜찮다. 
//will be SO

public abstract class Customer : MonoBehaviour
{
    public static readonly int maxCount = 5; //5명 이상은 존재 안할꺼다

    public CustomerType Type => data.Type;
    public CustomerJob Job => data.Job;

    public int BuyCount => data.buyCount;
    public float Frequency => data.frequency;
    public float BuyingTime => data.buyingTime;

    public CustomerEventHandler CustomerEvent;

    public int Gold => gold;

    [SerializeField] protected BuyPoint buyPoint;

    

    [Header("Customerinfo")]
    [SerializeField] CustomerData data;
    [SerializeField] protected Transform[] moveWayPoint;
    [SerializeField] protected int gold;


     


    

    protected CustomerState state;

    private Transform targetPos;
    protected Rigidbody2D rigid2D; //buyingLine 

    private Coroutine moveRoutine;

    protected virtual void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        CustomerEvent = new CustomerEventHandler();
    }

    protected virtual void Start()
    {

        

        StartCoroutine(CustomerFlow());
    }

    protected virtual void Update()
    { 
        
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
        Vector2 qPos = buyPoint.GetLastPosition();
        yield return MoveingWayPoint(qPos);
    }

   

    private IEnumerator JoinQueue()
    {
        state = CustomerState.InQueue;
        buyPoint.CustomerIn(this);
        yield return null; 
        
    }


    private IEnumerator WaitMyTurn()
    {
        state = CustomerState.WaitintTurn;
        yield return new WaitUntil(() => buyPoint.IsCustomFirst(this));

        CustomerEvent?.RaiseCustomerArrived(this.Job); //이벤트 연결
        yield return WaitForSecondsCache.Wait(data.buyingTime);
        
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
            while (Vector2.Distance(transform.position, wayPoint) > 0.1f)
            { 
            Vector2 dir = (wayPoint - (Vector2)transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime* data.moveSpeed);
            
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



}
