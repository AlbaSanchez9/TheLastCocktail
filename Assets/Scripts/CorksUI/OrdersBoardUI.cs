using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OrdersBoardUI : MonoBehaviour
{
    [SerializeField] private GameObject orderTicketPrefab;
    [SerializeField] private Transform ticketsParent;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private float ticketHeight = 0.15f;
    [SerializeField] private float startOffsetY = 0.3f;

    private List<GameObject> spawnedTickets = new List<GameObject>();

    private void Update()
    {
        RefreshOrders();
    }

    private void RefreshOrders()
    {
        foreach (GameObject t in spawnedTickets)
            Destroy(t);
        spawnedTickets.Clear();

        if (NetworkManager.Singleton.IsServer)
        {
            List<Customer> customers = customerManager.GetActiveCustomers();
            for (int i = 0; i < customers.Count; i++)
                SpawnTicket(i, customers[i].GetCocktailName(), customers[i].GetSnackName(),
                    customers[i].WantsDrink(), customers[i].WantsSnack());
        }
        else
        {
            var orders = customerManager.GetClientOrderData();
            for (int i = 0; i < orders.Count; i++)
                SpawnTicket(i, orders[i].cocktailName, orders[i].snackName,
                    orders[i].wantsDrink, orders[i].wantsSnack);
        }
    }

    private void SpawnTicket(int index, string cocktail, string snack, bool wantsDrink, bool wantsSnack)
    {
        GameObject ticketObj = Instantiate(orderTicketPrefab, ticketsParent);
        RectTransform rt = ticketObj.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(0, startOffsetY - index * ticketHeight, 0);
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        ticketObj.GetComponent<OrderTicketUI>().Setup(cocktail, snack, wantsDrink, wantsSnack);
        spawnedTickets.Add(ticketObj);
    }
}