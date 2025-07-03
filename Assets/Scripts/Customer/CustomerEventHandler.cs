using System;

public class CustomerEventHandler 
{
    public event Action<Customer> OnCustomerArrived;


    public void RaiseCustomerArrived(Customer customer) => OnCustomerArrived?.Invoke(customer);

    

}
