using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCustomer : Customer
{

    
    protected override void Start()
    {
        base.Start();
        
    }

   
    public override void Interact()
    {
        // 상점 시스템이 필요함
        //GameManager.Instance.Forge.AddGold(gold);
        CustomerManager.Instance.NotifyNormalCustomerPurchased(Job);
    }

    //Make Prefabs



    


}
