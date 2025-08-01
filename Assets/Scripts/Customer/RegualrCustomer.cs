using System;
using System.Collections;
using UnityEngine;

public class RegualrCustomer : Customer
{
    public Action<CustomerJob> OnPriceBoosted; // 단골손님 가격올리기

    [SerializeField] private GameObject InteractObject; // 말풍선
    [SerializeField] private float WaitTime = 4.0f;
    [SerializeField] private RegularCustomerData CollectData;

    private bool isDiscovered = false;
    private bool isInteracting = false;

    protected override void Start()
    {
        base.Start();
        if (isDiscovered && InteractObject != null)
        {
            InteractObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        //마인씬이면 동작 금지
        if (SceneCameraState.IsMineSceneActive)
            return;

        base.Update();
        if (isDiscovered)
            return;

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
        // 카메라 없으면 무시 (마인씬)
        if (Camera.main == null) return;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit == null || hit.transform != transform) return;

        isDiscovered = true;
        if (InteractObject != null)
            InteractObject.SetActive(false);

        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (gm.CollectionManager != null)
            {
                gm.CollectionManager.Discover(CollectData);
                gm.CollectionManager.AddVisited(CollectData.Key);
            }
            if (gm.DailyQuestManager != null)
            {
                gm.DailyQuestManager.ProgressQuest("InteractMissition", 1);
            }
        }
    }

    public override void Interact()
    {
        //아이에 사용 안됨
        OnPriceBoosted?.Invoke(Job); //가격 증가
        if (customerManager != null)
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
        if (buyPoint != null)
            buyPoint.CustomerOut();

        yield return MoveToExit();
    }

    private IEnumerator MoveToExit()
    {
        state = CustomerState.Exiting;
        if (moveWayPoint != null && moveWayPoint.Length > 1)
            yield return MoveingWayPoint(moveWayPoint[1].position);
        CustomerExit();
    }
}
