using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float spawnInterval = 20f;

    [SerializeField] private OrderManager orderManager;

    [SerializeField] private OrderUI orderUI;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;
        }
    }

    private void SpawnCustomer()
    {
        GameObject obj = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);

        Customer customer = obj.GetComponent<Customer>();

        Order order = orderManager.GenerateOrder();

        customer.SetOrder(order);

        orderUI.UpdateOrder(order.Recipe.CocktailName);
    }
}