using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomerManager : NetworkBehaviour
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
    [SerializeField] private float barCustomerChance = 0.5f;
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private Transform exitTransform;

    private float timer;
    private List<Customer> barCustomers = new List<Customer>();
    private List<Customer> tableCustomers = new List<Customer>();

    public struct ClientOrderInfo
    {
        public string cocktailName;
        public string snackName;
        public bool wantsDrink;
        public bool wantsSnack;
    }

    private List<ClientOrderInfo> clientOrderData = new List<ClientOrderInfo>();
    public List<ClientOrderInfo> GetClientOrderData() => clientOrderData;

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!RoundManager.Instance.IsRoundActive()) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;
            spawnInterval = Mathf.Max(4f, spawnInterval - 0.3f);
        }
    }

    private void Start()
    {
        barCustomers.Clear();
        tableCustomers.Clear();
        timer = 0f;
        spawnInterval = 20f;
        clientOrderData.Clear();
    }

    private void SpawnCustomer()
    {
        bool spawnAtBar = Random.value < barCustomerChance;
        if (spawnAtBar) TrySpawnAt(barSpots, barCustomers, barCustomerPrefab, false);
        else TrySpawnAt(tableSpots, tableCustomers, tableCustomerPrefab, true);
    }

    private void TrySpawnAt(List<Transform> spots, List<Customer> list, GameObject prefab, bool isTable)
    {
        if (list.Count >= spots.Count) return;
        Transform freeSpot = GetFreeSpot(spots, list);
        if (freeSpot == null) return;

        GameObject obj = Instantiate(prefab, freeSpot.position, freeSpot.rotation);
        obj.GetComponent<NetworkObject>().Spawn();

        Customer customer = obj.GetComponent<Customer>();
        customer.SetOrder(orderManager.GenerateOrder());
        customer.SetSnackOrder((SnackType)Random.Range(0, System.Enum.GetValues(typeof(SnackType)).Length));
        customer.SetManager(this);
        customer.SetExitPoint(exitTransform);

        if (isTable && tableCenters.Count > 0)
            customer.SetTableCenter(GetNearestTableCenter(freeSpot.position));

        list.Add(customer);
        StartCoroutine(SyncAfterDelay()); // ← delay para esperar al Start del cliente
    }

    private IEnumerator SyncAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // espera a que Start() haya corrido
        SyncOrdersToClients();
    }

    private void SyncOrdersToClients()
    {
        if (!IsServer) return;
        if (!IsSpawned) return;

        var all = GetActiveCustomers();
        if (all.Count == 0)
        {
            SyncOrdersClientRpc("", "", "", "");
            return;
        }

        string[] cocktails = new string[all.Count];
        string[] snacks = new string[all.Count];
        string[] drinks = new string[all.Count];
        string[] snackWants = new string[all.Count];

        for (int i = 0; i < all.Count; i++)
        {
            cocktails[i] = all[i].GetCocktailName();
            snacks[i] = all[i].GetSnackName();
            drinks[i] = all[i].WantsDrink() ? "1" : "0";
            snackWants[i] = all[i].WantsSnack() ? "1" : "0";
        }

        SyncOrdersClientRpc(
            string.Join("|", cocktails),
            string.Join("|", snacks),
            string.Join("|", drinks),
            string.Join("|", snackWants)
        );
    }

    [ClientRpc]
    private void SyncOrdersClientRpc(string cocktailsJoined, string snacksJoined, string drinksJoined, string snackWantsJoined)
    {
        if (IsServer) return;

        clientOrderData.Clear();
        if (cocktailsJoined.Length == 0) return;

        string[] cocktails = cocktailsJoined.Split('|');
        string[] snacks = snacksJoined.Split('|');
        string[] drinks = drinksJoined.Split('|');
        string[] snackWants = snackWantsJoined.Split('|');

        for (int i = 0; i < cocktails.Length; i++)
        {
            clientOrderData.Add(new ClientOrderInfo
            {
                cocktailName = cocktails[i],
                snackName = snacks[i],
                wantsDrink = drinks[i] == "1",
                wantsSnack = snackWants[i] == "1"
            });
        }
    }

    private Transform GetNearestTableCenter(Vector3 position)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (Transform center in tableCenters)
        {
            float dist = Vector3.Distance(position, center.position);
            if (dist < minDist) { minDist = dist; nearest = center; }
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
                if (Vector3.Distance(c.transform.position, spot.position) < 0.1f) { occupied = true; break; }
            if (!occupied) freeSpots.Add(spot);
        }
        return freeSpots.Count == 0 ? null : freeSpots[Random.Range(0, freeSpots.Count)];
    }

    public void CustomerLeft(Customer customer)
    {
        barCustomers.Remove(customer);
        tableCustomers.Remove(customer);
        SyncOrdersToClients();
    }

    public List<Customer> GetActiveCustomers()
    {
        var all = new List<Customer>();
        all.AddRange(barCustomers);
        all.AddRange(tableCustomers);
        return all;
    }
}