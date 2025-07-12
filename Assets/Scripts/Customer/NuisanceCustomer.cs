using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NuisanceCustomer : Customer
{
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private int penaltyGold;
    [SerializeField] private GameObject InteractIcon;
    [SerializeField] private float offsetY = 0.5f;

    [Header("ExitPoint")]
    [SerializeField] private Transform exitPoint;


    private bool isClicked = false;

    


    protected override void Start()
    {
        
        if (buyPoint == null)
        {
            buyPoint = GetRandomBuyPoint();
        }
        if (InteractIcon != null)
        {
            InteractIcon.SetActive(true);
        }

      //  StartCoroutine(NuisanceFlow());
      
    }
    protected override void OnEnable()
    {
        
        isClicked = false;
        if (InteractIcon != null)
        {
            InteractIcon.SetActive(true);
        }
        base.OnEnable();
    }

    protected override IEnumerator CustomerFlow()
    {
        yield return MoveRandomPlace();

        float timer = 0f;
        while (timer < waitTime && !isClicked)
        {
            timer += Time.deltaTime;
            yield return null;
        }
               
        yield return ExitFlow();
    }



    protected override void Update()
    {
        base.Update();
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭됨");
            CheckClick(Input.mousePosition);
        }

#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        CheckClick(Input.GetTouch(0).position);
    }
#endif 


    }

    private void CheckClick(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.transform == transform)
        {
            Interact();
           
        }
    }
    public override void Interact()
    {
        if (isClicked)
        {
            return;
        }
        isClicked = true;
        if (InteractIcon != null)
        {
            Debug.Log("InteractIcon 비활성화 시도");
            InteractIcon.SetActive(false);
        }


        StopAllCoroutines();
        StartCoroutine(ExitFlow());

    }

    private IEnumerator ExitFlow()       
    {
        state = CustomerState.Exiting;
                
        if (moveWayPoint != null && moveWayPoint.Length > 0 && moveWayPoint[0] != null)
        {
            yield return MoveingWayPoint(moveWayPoint[0].position);
        }
        else
        {
            Debug.LogError($"[NuisanceCustomer] 퇴장 경로(moveWayPoint)가 설정되지 않았습니다!", this.gameObject);
        }
        
        if (!isClicked)
        {
            PenaltyGold();
        }
              
        CustomerExit();
    }
  

    //private 

    private BuyPoint GetRandomBuyPoint() //수정해야될듯
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
        GameManager.Instance.Forge.AddGold(-1000);
        Debug.Log("골드 차감");
    }
}
