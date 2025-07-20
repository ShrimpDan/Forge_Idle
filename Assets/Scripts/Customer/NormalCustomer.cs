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
        customerManager.NotifyNormalCustomerPurchased(Job);
    }
}
