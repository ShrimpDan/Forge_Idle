using System.Collections.Generic;
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


    //마지막 위치를 반환해줘야 할듯한데
    public Vector2 GetLastPosition()
    {
        int index = waitingCustomers.Count;
        if (index >= queuePos.Length)
        {
            return queuePos[queuePos.Length - 1].position;//맨끝 반환
        }

        return queuePos[index].position;
    }


}
