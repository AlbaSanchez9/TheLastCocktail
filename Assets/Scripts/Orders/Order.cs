using UnityEngine;

[System.Serializable]
public class Order
{
    [SerializeField] private CocktailRecipe recipe;

    public CocktailRecipe Recipe => recipe;

    public Order(CocktailRecipe recipe)
    {
        this.recipe = recipe;
    }
}