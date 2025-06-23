using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCustomer : Customer
{
    protected virtual void Start()
    {
        base.Start();
        
    }

   
    public override void Interact()
    {
        Debug.Log("시작");
    }

    //Make Prefabs



    


}
