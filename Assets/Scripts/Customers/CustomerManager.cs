using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject barCustomerPrefab;
    [SerializeField] private GameObject tableCustomerPrefab;

    [Header("Spots")]
    [SerializeField] private List<Transform> barSpots;
    [SerializeField] private List<Transform> tableSpots;

    [Header("Mesas")]
    [SerializeField] private List<Transform> tableCenters;

    [Header("Config")]
    [SerializeField] private float spawnInterval = 20f;
    [SerializeField] private float barCustomerChance = 0.5f; // 0.5 = 50% barra, 50% mesa
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private Transform exitTransform;

    private float timer;
    private List<Customer> barCustomers = new List<Customer>();
    private List<Customer> tableCustomers = new List<Customer>();

    private void Update()
    {
        if (!RoundManager.Instance.IsRoundActive()) return;

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
        //if (!RoundManager.Instance.IsRoundActive()) return;

        //if (customers.Count >= queueSpots.Count)
        //    return;

        //Transform freeSpot = GetFreeSpot();
        //if (freeSpot == null)
        //    return;

        //GameObject obj = Instantiate(customerPrefab, freeSpot.position, freeSpot.rotation);
        //Customer customer = obj.GetComponent<Customer>();

        //Order order = orderManager.GenerateOrder();
        //customer.SetOrder(order);

        //SnackType randomSnack = (SnackType)Random.Range(0, System.Enum.GetValues(typeof(SnackType)).Length);
        //customer.SetSnackOrder(randomSnack);

        //customers.Add(customer);
        //customer.SetManager(this);
        //customer.SetExitPoint(exitTransform);

        bool spawnAtBar = Random.value < barCustomerChance;

        //if (spawnAtBar)
        //    TrySpawnAt(barSpots, barCustomers, barCustomerPrefab);
        //else
        //    TrySpawnAt(tableSpots, tableCustomers, tableCustomerPrefab);

        if (spawnAtBar)
            TrySpawnAt(barSpots, barCustomers, barCustomerPrefab, false);
        else
            TrySpawnAt(tableSpots, tableCustomers, tableCustomerPrefab, true);
    }

    //private void TrySpawnAt(List<Transform> spots, List<Customer> list, GameObject prefab)
    //{
    //    if (list.Count >= spots.Count) return;

    //    Transform freeSpot = GetFreeSpot(spots, list);
    //    if (freeSpot == null) return;

    //    GameObject obj = Instantiate(prefab, freeSpot.position, freeSpot.rotation);
    //    Customer customer = obj.GetComponent<Customer>();

    //    customer.SetOrder(orderManager.GenerateOrder());
    //    customer.SetSnackOrder((SnackType)Random.Range(0, System.Enum.GetValues(typeof(SnackType)).Length));
    //    customer.SetManager(this);
    //    customer.SetExitPoint(exitTransform);

    //    list.Add(customer);
    //}

    private void TrySpawnAt(List<Transform> spots, List<Customer> list, GameObject prefab, bool isTable)
    {
        if (list.Count >= spots.Count) return;

        Transform freeSpot = GetFreeSpot(spots, list);
        if (freeSpot == null) return;

        GameObject obj = Instantiate(prefab, freeSpot.position, freeSpot.rotation);
        Customer customer = obj.GetComponent<Customer>();

        customer.SetOrder(orderManager.GenerateOrder());
        customer.SetSnackOrder((SnackType)Random.Range(0, System.Enum.GetValues(typeof(SnackType)).Length));
        customer.SetManager(this);
        customer.SetExitPoint(exitTransform);

        // Solo para clientes de mesa
        if (isTable && tableCenters.Count > 0)
        {
            Transform nearestCenter = GetNearestTableCenter(freeSpot.position);
            customer.SetTableCenter(nearestCenter);
        }

        list.Add(customer);
    }

    private Transform GetNearestTableCenter(Vector3 position)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (Transform center in tableCenters)
        {
            float dist = Vector3.Distance(position, center.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = center;
            }
        }
        return nearest;
    }

    private Transform GetFreeSpot(List<Transform> spots, List<Customer> list)
    {
        List<Transform> freeSpots = new List<Transform>();

        foreach (Transform spot in spots)
        {
            bool occupied = false;
            foreach (Customer c in list)
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

        return freeSpots.Count == 0 ? null : freeSpots[Random.Range(0, freeSpots.Count)];
    }

    //private Transform GetFreeSpot()
    //{
    //    List<Transform> freeSpots = new List<Transform>();

    //    foreach (Transform spot in queueSpots)
    //    {
    //        bool occupied = false;

    //        foreach (Customer c in customers)
    //        {
    //            if (Vector3.Distance(c.transform.position, spot.position) < 0.1f)
    //            {
    //                occupied = true;
    //                break;
    //            }
    //        }

    //        if (!occupied)
    //            freeSpots.Add(spot);
    //    }

    //    if (freeSpots.Count == 0)
    //        return null;

    //    return freeSpots[Random.Range(0, freeSpots.Count)];
    //}

    public void CustomerLeft(Customer customer)
    {
        barCustomers.Remove(customer);
        tableCustomers.Remove(customer);
    }
}