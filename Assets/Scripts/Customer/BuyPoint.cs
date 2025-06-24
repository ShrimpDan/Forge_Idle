using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BuyPoint : MonoBehaviour
{
    [SerializeField] private Transform[] queuePos;
    Queue<Customer> waitingCustomers = new Queue<Customer>();


    public void CustomerIn(Customer newCustomer)
    {
        waitingCustomers.Enqueue(newCustomer);
        UpdateQueuePosition();
    }


    public void CustomerOut()
    {
        if (waitingCustomers.Count > 0)
        {
            waitingCustomers.Dequeue();
            UpdateQueuePosition();
        }

    }

    private void UpdateQueuePosition()
    {
        int index = 0;
        foreach (var customer in waitingCustomers)
        {
            if (index < queuePos.Length)
            { 
            customer.SetQueuePos(queuePos[index].position);
            }
            index++;

        }
    }

    public bool IsCustomFirst(Customer customer)
    {
        return waitingCustomers.Count > 0 && waitingCustomers.Peek() == customer;
        
    }





}
