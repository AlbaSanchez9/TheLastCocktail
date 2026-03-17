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
        if (!RoundManager.Instance.IsRoundActive()) return;

        Glass glass = other.GetComponent<Glass>();
        if (glass != null)
        {
            string cocktail = recipeManager.CheckGlass(glass);
            customer.TryServeDrink(cocktail, glass);
            return;
        }

        SnackPrefab snack = other.GetComponent<SnackPrefab>();
        if (snack != null)
        {
            bool correct = customer.TryServeSnack(snack.snackType);
            if (correct)
                Destroy(other.gameObject);
        }
    }
}