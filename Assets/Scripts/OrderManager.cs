using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private List<CocktailRecipe> possibleRecipes;

    private Order currentOrder;

    private void Start()
    {
        GenerateOrder();
    }

    public void GenerateOrder()
    {
        int index = Random.Range(0, possibleRecipes.Count);
        currentOrder = new Order(possibleRecipes[index]);

        Debug.Log("Nuevo pedido: " + currentOrder.Recipe.CocktailName);
    }

    public Order GetCurrentOrder()
    {
        return currentOrder;
    }
}