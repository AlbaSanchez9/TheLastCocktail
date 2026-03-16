using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    private List<string> ingredients = new List<string>();

    [SerializeField] private bool isDirty;

    public void AddIngredient(string ingredient)
    {
        if (isDirty) return;

        ingredients.Add(ingredient);
        Debug.Log("Ingredientes actuales: " + string.Join(", ", ingredients));
    }

    public IReadOnlyList<string> GetIngredients()
    {
        return ingredients;
    }

    public void MakeDirty()
    {
        isDirty = true;
        ingredients.Clear();
    }

    public void Clean()
    {
        isDirty = false;
        ingredients.Clear();  
    }

    public bool IsDirty()
    {
        return isDirty;
    }

    //public string CheckRecipe(List<CocktailRecipe> recipes)
    //{
    //    foreach (var recipe in recipes)
    //    {
    //        if (MatchRecipe(recipe))
    //        {
    //            return recipe.CocktailName;
    //        }
    //    }

    //    return null;
    //}

    //private bool MatchRecipe(CocktailRecipe recipe)
    //{
    //    var recipeIngredients = recipe.Ingredients;

    //    if (recipeIngredients.Count != ingredients.Count)
    //        return false;

    //    foreach (var ingredient in recipeIngredients)
    //    {
    //        if (!ingredients.Contains(ingredient))
    //            return false;
    //    }

    //    return true;
    //}
}