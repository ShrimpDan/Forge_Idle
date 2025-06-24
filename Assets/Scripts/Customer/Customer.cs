using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum CustomerType
{
    Normal,
    Regular,
    Nuisance

}

public enum CustomerJob
{
    Woodcutter,
    Farmer,
    Miner,
    Warrior,
    Archer,
    Assassin
}

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
    public CustomerType Type => type;
    public CustomerJob Job => job;
    public CustomerState State => state;
    public int BuyCount => buyCount;

    [SerializeField] private BuyPoint buyPoint;

    [Header("Customerinfo")]
    [SerializeField] private CustomerType type;
    [SerializeField] private CustomerJob job;
    [SerializeField] private Transform[] moveWayPoint;
    [SerializeField] private int buyCount;
    [SerializeField] private float Frequency;
    [SerializeField] private float buyingTime = 1.5f;
    [SerializeField] private float moveSpeed = 3f;

    private bool isMoving = false;
    private CustomerState state;

    private Transform targetPos;
    protected Rigidbody2D rigid2D; //buyingLine 

    private Coroutine moveRoutine;

    protected virtual void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
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
        //코루틴을 세분화 시킬꺼임

        yield return MoveToBuyZone();
        yield return JoinQueue();
        yield return WaitMyTurn();
        yield return PerformPurChase();
        yield return MoveToExit();
    }


    private IEnumerator MoveToBuyZone()
    {
        state = CustomerState.MovingToBuyZone;
        yield return MoveingWayPoint(moveWayPoint[0].position);
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
        yield return new WaitForSeconds(buyingTime);
        
    }

    private IEnumerator PerformPurChase()
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
    
    }


    private IEnumerator MoveingWayPoint(Vector2 wayPoint)
    {
            while (Vector2.Distance(transform.position, wayPoint) > 0.1f)
            { 
            Vector2 dir = (wayPoint - (Vector2)transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime* moveSpeed);
            
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
    

    public abstract void Interact();



}
