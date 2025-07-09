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
        CustomerManager.Instance.NotifyNormalCustomerPurchased(Job);
    }

    //Make Prefabs



    


}
