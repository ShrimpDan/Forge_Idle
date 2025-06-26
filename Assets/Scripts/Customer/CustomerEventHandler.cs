using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerEventHandler 
{
    public event Action<CustomerJob> OnCustomerArrived;


    public void RaiseCustomerArrived(CustomerJob job) => OnCustomerArrived(job);

    

}
