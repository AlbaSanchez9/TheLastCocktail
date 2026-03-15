using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<CocktailRecipe> recipes;

    public string CheckGlass(Glass glass)
    {
        return glass.CheckRecipe(recipes);
    }
}