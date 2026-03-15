using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private List<CocktailRecipe> possibleRecipes;

    public Order GenerateOrder()
    {
        int index = Random.Range(0, possibleRecipes.Count);

        Order newOrder = new Order(possibleRecipes[index]);

        Debug.Log("Nuevo pedido: " + newOrder.Recipe.CocktailName);

        return newOrder;
    }
}