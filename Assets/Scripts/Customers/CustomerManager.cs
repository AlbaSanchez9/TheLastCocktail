using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private List<Transform> queueSpots;

    [SerializeField] private float spawnInterval = 20f;

    [SerializeField] private OrderManager orderManager;

    [SerializeField] private Transform exitTransform;

    private float timer;

    private List<Customer> customers = new List<Customer>();

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;

            // dificultad creciente
            spawnInterval = Mathf.Max(4f, spawnInterval - 0.3f);
        }
    }

    private void SpawnCustomer()
    {
        if (!RoundManager.Instance.IsRoundActive()) return;

        if (customers.Count >= queueSpots.Count)
            return;

        Transform freeSpot = GetFreeSpot();
        if (freeSpot == null)
            return;

        GameObject obj = Instantiate(customerPrefab, freeSpot.position, freeSpot.rotation);
        Customer customer = obj.GetComponent<Customer>();

        Order order = orderManager.GenerateOrder();
        customer.SetOrder(order);

        SnackType randomSnack = (SnackType)Random.Range(0, System.Enum.GetValues(typeof(SnackType)).Length);
        customer.SetSnackOrder(randomSnack);

        customers.Add(customer);
        customer.SetManager(this);
        customer.SetExitPoint(exitTransform);
    }

    private Transform GetFreeSpot()
    {
        List<Transform> freeSpots = new List<Transform>();

        foreach (Transform spot in queueSpots)
        {
            bool occupied = false;

            foreach (Customer c in customers)
            {
                if (Vector3.Distance(c.transform.position, spot.position) < 0.1f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                freeSpots.Add(spot);
        }

        if (freeSpots.Count == 0)
            return null;

        return freeSpots[Random.Range(0, freeSpots.Count)];
    }

    public void CustomerLeft(Customer customer)
    {
        customers.Remove(customer);
    }
}