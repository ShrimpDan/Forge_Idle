using System;
using System.Collections;
using UnityEngine;

public class RegualrCustomer : Customer
{


    public Action<CustomerJob> OnPriceBoosted;//단골손님 가격올리기

    [SerializeField] private GameObject InteractObject; //말풍선
    [SerializeField] private float WaitTime = 4.0f;
    [SerializeField] private RegularCustomerData CollectData;


    private bool isDiscovered = false;
    private bool isInteracting = false;

    protected override void Start()
    {
        base.Start();
        if (isDiscovered)
        {
            InteractObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isDiscovered)
        {
            return;
        }

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
            isDiscovered = true;
            InteractObject.SetActive(false);
            GameManager.Instance.CollectionManager.Discover(CollectData);
           GameManager.Instance.CollectionManager.AddVisited(CollectData.Key); //클릭시 방문 횟수 채크
            GameManager.Instance.DailyQuestManager.ProgressQuest("InteractMissition", 1);


        }
    }

    public override void Interact()
    {
        //아이에 사용 안됨
        OnPriceBoosted?.Invoke(Job); //가격 증가
        customerManager.NotifyNormalCustomerPurchased(Job);
    }


    public void SettingCollectData(RegularCustomerData data)
    {
        CollectData = data;
    }



    protected override IEnumerator PerformPurChase()
    {
        state = CustomerState.Purchasing;
        Interact(); // 가격 상승 등 효과
        buyPoint.CustomerOut();

        yield return MoveToExit();
     

    }

    private IEnumerator MoveToExit()
    {
        state = CustomerState.Exiting;
        yield return MoveingWayPoint(moveWayPoint[1].position);
        CustomerExit();
    }

    
}
