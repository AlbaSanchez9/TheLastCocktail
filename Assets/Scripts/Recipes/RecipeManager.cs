using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<CocktailRecipe> recipes;

    public string CheckGlass(Glass glass)
    {
        var ingredients = glass.GetIngredients();

        Debug.Log("Ingredientes en vaso: " + string.Join(", ", ingredients));

        foreach (var recipe in recipes)
        {
            if (MatchRecipe(recipe, ingredients))
                return recipe.CocktailName;
        }

        return null;
    }

    private bool MatchRecipe(CocktailRecipe recipe, IReadOnlyList<string> ingredients)
    {
        if (recipe.Ingredients.Count != ingredients.Count)
            return false;

        foreach (var ingredient in recipe.Ingredients)
        {
            //if (!ingredients.Contains(ingredient))
            //    return false; ////Sin importar el orden de los ingredientes

            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                if (recipe.Ingredients[i] != ingredients[i])
                    return false;
            }
        }

        return true;
    }
}