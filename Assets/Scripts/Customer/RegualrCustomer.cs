using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegualrCustomer : Customer
{

    public Action<CustomerJob> OnPriceBoosted;//단골손님 가격올리기

    [SerializeField] private GameObject InteractObject; //말풍선
    [SerializeField] private float WaitTime = 4.0f;
    [SerializeField] private RegualrCustomerData CollectData;


    private bool isInteracting = false;


    public override void Interact()
    {
        if (!isInteracting)
        {
            //여기서 이제 버프 효과 주면 될듯
        }

    }






    protected override IEnumerator PerformPurChase()
    {
        InteractObject.SetActive(true);
        if (isInteracting) yield break;
        isInteracting = true;
        state = CustomerState.Purchasing;
        float time = 0f;
        bool click = false;

        while (time < WaitTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                click = true;
            }

            time += Time.deltaTime;
            yield return null;

        }

        if (click)
        {
            RegualrEvent();
            InteractObject.SetActive(false);
            buyPoint.CustomerOut();
        }
        buyPoint.CustomerOut(); //이어서 나가기
        isInteracting = false;

    }

    private void RegualrEvent()
    {
        
        OnPriceBoosted?.Invoke(Job); //단골손님 가격 오르기 이벤트 발생
        if (CollectData != null)
        {
            CollectionBook.Instance.Discover(CollectData);
        }
        else
        {

            Debug.Log("컬렉션 북이 연결 안됨");
        }
    }

    
}
