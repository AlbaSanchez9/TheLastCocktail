using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private RecipeManager recipeManager;

    private void OnTriggerEnter(Collider other)
    {
        Glass glass = other.GetComponent<Glass>();

        if (glass == null) return;

        string cocktail = recipeManager.CheckGlass(glass);

        Order currentOrder = orderManager.GetCurrentOrder();

        if (cocktail != null && cocktail == currentOrder.Recipe.CocktailName)
        {
            Debug.Log("Pedido correcto!");
            orderManager.GenerateOrder();
        }
        else
        {
            Debug.Log("Pedido incorrecto!");
        }
    }
}