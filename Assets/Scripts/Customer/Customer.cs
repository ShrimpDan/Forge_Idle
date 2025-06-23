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

//will be SO

public abstract class Customer : MonoBehaviour
{
    public static readonly int maxCount = 5; //5명 이상은 존재 안할꺼다
    public CustomerType Type => type;
    public CustomerJob Job => job;
    public int BuyCount => buyCount;

    [SerializeField] private BuyPoint buyPoint;

    [Header("Customerinfo")]
    [SerializeField] private CustomerType type;
    [SerializeField] private CustomerJob job;
    [SerializeField] private Transform[] moveWayPoint;
    [SerializeField] private int buyCount;
    [SerializeField] private float Frequency;


    private bool isMoving = false;
  
    private Transform targetPos;
    protected Rigidbody2D rigid2D; //buyingLine 

    protected virtual void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        targetPos = moveWayPoint[0]; //시작점 
        
        StartCoroutine(MoveCustomerRoutine());
    }

    protected virtual void Update()
    { 
        
    }

   

    private IEnumerator MoveCustomerRoutine()
    {
        int wayPointIndex = 0;
        Vector2 point = moveWayPoint[wayPointIndex].position;
        yield return MoveingWayPoint(point);

        Debug.Log("구매구역 도착 완료");

        //
        wayPointIndex++;
        point = moveWayPoint[wayPointIndex].position;
        //구매 함수 호출
        buyPoint.CustomerIn(this);
        yield return new WaitUntil(() => buyPoint.IsCustomFirst(this));
        Debug.Log($"내가 1번임{gameObject.name}");

        Interact();




        yield return MoveingWayPoint((Vector2)moveWayPoint[wayPointIndex].position);

    }





    private IEnumerator MoveingWayPoint(Vector2 wayPoint)
    {
            while (Vector2.Distance(transform.position, wayPoint) > 0.1f)
            { 
            Vector2 dir = (wayPoint - (Vector2)transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime);
            
            yield return new WaitForFixedUpdate();
            }
    }


    public void SetQueuePos(Vector2 queuePos)
    {
        StopAllCoroutines();//다른 모든 코루틴을끄고
        StartCoroutine(MoveingWayPoint(queuePos));
    }
    

    public abstract void Interact();



}
