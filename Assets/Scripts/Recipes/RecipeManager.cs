using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    //[SerializeField] private List<CocktailRecipe> recipes;

    //public string CheckGlass(Glass glass)
    //{
    //    var ingredients = glass.GetIngredients();

    //    Debug.Log("Ingredientes en vaso: " + string.Join(", ", ingredients));

    //    foreach (var recipe in recipes)
    //    {
    //        if (MatchRecipe(recipe, glass))
    //            return recipe.CocktailName;
    //    }

    //    return null;
    //}

    //private bool MatchRecipe(CocktailRecipe recipe, IReadOnlyList<string> ingredients)
    //{
    //    if (recipe.Ingredients.Count != ingredients.Count)
    //        return false;

    //    foreach (var ingredient in recipe.Ingredients)
    //    {
    //        //if (!ingredients.Contains(ingredient))
    //        //    return false; ////Sin importar el orden de los ingredientes

    //        for (int i = 0; i < recipe.Ingredients.Count; i++)
    //        {
    //            if (recipe.Ingredients[i] != ingredients[i])
    //                return false;
    //        }
    //    }

    //    return true;
    //}

    [SerializeField] private List<CocktailRecipe> recipes;

    public string CheckGlass(Glass glass)
    {
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(recipe, glass))
            {
                glass.SetDrinkColor(recipe.DrinkColor);
                return recipe.CocktailName;
            }
        }
        return null;
    }

    public Color CheckGlassColor(Glass glass)
    {
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(recipe, glass))
                return recipe.DrinkColor;
        }
        return Color.clear;
    }

    private bool MatchRecipe(CocktailRecipe recipe, Glass glass)
    {
        var glassLiquids = glass.GetLiquidIngredients();
        var glassSolids = glass.GetSolidIngredients();

        if (glassLiquids.Count != recipe.LiquidIngredients.Count) return false;
        if (glassSolids.Count != recipe.SolidIngredients.Count) return false;

        return glassLiquids.OrderBy(x => x).SequenceEqual(recipe.LiquidIngredients.OrderBy(x => x)) &&
               glassSolids.OrderBy(x => x).SequenceEqual(recipe.SolidIngredients.OrderBy(x => x));
    }
}