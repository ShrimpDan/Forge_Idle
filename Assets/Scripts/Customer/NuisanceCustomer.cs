using System.Collections;
using UnityEngine;

public class NuisanceCustomer : Customer
{
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private int penaltyGold;
    [SerializeField] private GameObject InteractIcon;
    [SerializeField] private float offsetY = 0.5f;

    [Header("ExitPoint")]
    [SerializeField] private Transform exitPoint;


    private bool clicked = false;

    protected override void Start()
    {
        
        if (buyPoint == null)
        {
            buyPoint = GetRandomBuyPoint();
        }
        StartCoroutine(NuisanceFlow());
      
    }
    public override void Interact()
    {
        if (clicked)
        {
            return;
        }
        clicked = true;
        InteractIcon?.SetActive(false);


        StopAllCoroutines();
        StartCoroutine(ExitFlow());

    }


    private IEnumerator NuisanceFlow()
    {
       
        if (buyPoint == null)
        {
            Debug.Log("포인트가 없습니다");
            CustomerExit();
            yield break;
        }

        Vector2 pointPos = (Vector2)buyPoint.transform.position + Vector2.down * offsetY;
        yield return StartCoroutine(MoveingWayPoint(pointPos));


        yield return StartCoroutine(WaitForInteraction());

      
        yield return StartCoroutine(ExitFlow());

    }


    private IEnumerator WaitForInteraction()
    {
        if (InteractIcon != null)
        {
            InteractIcon.SetActive(true);
        }

        float time = 0f;
        clicked = false;

        while (time < waitTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked = true;
                break;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ExitFlow()       
    {
        state = CustomerState.Exiting;
        if (exitPoint != null)
        { 
        yield return StartCoroutine(MoveingWayPoint(exitPoint.position));
        }
      

        if (!clicked)
        { 
            PenaltyGold();
        }

        CustomerExit();
    }



    private BuyPoint GetRandomBuyPoint()
    {
        var points = CustomerManager.Instance.allBuyPoints; //넣어줄 예정
        if (points == null || points.Count == 0)
        {
            return null;
        }
        return points[Random.Range(0, points.Count)];

    }

    private void PenaltyGold()
    {
        GameManager.Instance.Forge.AddGold(500);
        Debug.Log("골드 차감");
    }
}
