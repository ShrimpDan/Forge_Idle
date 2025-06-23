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
    public CustomerType Type => type;
    public CustomerJob Job => job;
    public int BuyCount => buyCount;


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
        targetPos = moveWayPoint[0];
        
        StartCoroutine(MoveCustomerRoutine());
    }

    protected virtual void Update()
    { 
        
    }

    private void ChangeWayPoint(Transform wayPoint)
    {
        targetPos = wayPoint;
        isMoving = true;
    }

    private IEnumerator MoveCustomerRoutine()
    {
        int wayPointIndex = 0;
        yield return MoveingWayPoint();

        Debug.Log("구매구역 도착 완료");

        //
        wayPointIndex++;
        //구매 함수 호출


        ChangeWayPoint(moveWayPoint[wayPointIndex]);
        if (wayPointIndex > moveWayPoint.Length)
        {
            wayPointIndex = moveWayPoint.Length;
        }



        yield return MoveingWayPoint();

    }





    private IEnumerator MoveingWayPoint()
    {
      
            while (Vector2.Distance(transform.position, targetPos.position) > 0.1f)
            { 
            Vector2 dir = (targetPos.position - transform.position).normalized;
            rigid2D.MovePosition(rigid2D.position + dir * Time.deltaTime);
            
            yield return new WaitForFixedUpdate();
            }
        




    }

    

    public abstract void Interact();



}
