using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private Customer customer;
    private RecipeManager recipeManager;

    private void Awake()
    {
        customer = GetComponentInParent<Customer>();
        recipeManager = FindFirstObjectByType<RecipeManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Glass glass = other.GetComponent<Glass>();

        if (glass == null) return;

        string cocktail = recipeManager.CheckGlass(glass);

        customer.TryServeDrink(cocktail, glass);
    }
}