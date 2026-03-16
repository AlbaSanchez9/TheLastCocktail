using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<Transform> queueSpots;

    [SerializeField] private float spawnInterval = 20f;

    [SerializeField] private OrderManager orderManager;

    [SerializeField] private OrderUI orderUI;

    private float timer;

    private List<Customer> customers = new List<Customer>();

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;
            spawnInterval = Mathf.Max(4f, spawnInterval - 0.3f);
        }
    }

    private void SpawnCustomer()
    {
        if (customers.Count >= queueSpots.Count)
            return;

        Transform spot = queueSpots[customers.Count];

        GameObject obj = Instantiate(customerPrefab, spot.position, spot.rotation);

        Customer customer = obj.GetComponent<Customer>();

        Order order = orderManager.GenerateOrder();

        customer.SetOrder(order);

        customers.Add(customer);

        orderUI.UpdateOrder(order.Recipe.CocktailName);
    }

    public void CustomerLeft(Customer customer)
    {
        customers.Remove(customer);

        ReorganizeQueue();
    }

    private void ReorganizeQueue()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            customers[i].MoveTo(queueSpots[i]);
        }
    }
}