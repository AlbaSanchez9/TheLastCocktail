using System.Collections.Generic;
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

        List<Customer> customers = customerManager.GetActiveCustomers();

        for (int i = 0; i < customers.Count; i++)
        {
            GameObject ticketObj = Instantiate(orderTicketPrefab, ticketsParent);

            RectTransform rt = ticketObj.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, startOffsetY - i * ticketHeight, 0);
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            OrderTicketUI ticket = ticketObj.GetComponent<OrderTicketUI>();
            ticket.Setup(
                customers[i].GetCocktailName(),
                customers[i].GetSnackName(),
                customers[i].WantsDrink(),
                customers[i].WantsSnack()
            );

            spawnedTickets.Add(ticketObj);
        }
    }
}