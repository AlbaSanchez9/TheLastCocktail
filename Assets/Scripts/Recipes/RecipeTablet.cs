using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeTablet : MonoBehaviour
{
    [Header("Recetas")]
    [SerializeField] private List<CocktailRecipe> recipes;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI cocktailNameText;
    [SerializeField] private TextMeshProUGUI ingredientsText;
    [SerializeField] private Image cocktailImage;
    [SerializeField] private TextMeshProUGUI pageIndicatorText; 

    private int currentIndex = 0;

    private void Start()
    {
        if (recipes.Count > 0)
            ShowRecipe(0);
    }

    public void NextRecipe()
    {
        if (recipes.Count == 0) return;
        currentIndex = (currentIndex + 1) % recipes.Count;
        ShowRecipe(currentIndex);
    }

    public void PreviousRecipe()
    {
        if (recipes.Count == 0) return;
        currentIndex = (currentIndex - 1 + recipes.Count) % recipes.Count;
        ShowRecipe(currentIndex);
    }

    private void ShowRecipe(int index)
    {
        CocktailRecipe recipe = recipes[index];

        if (cocktailNameText != null)
            cocktailNameText.text = recipe.CocktailName;

        if (ingredientsText != null)
        {
            string liquids = "Líquidos:\n" + string.Join("\n", recipe.LiquidIngredients);
            string solids = "\nSólidos:\n" + string.Join("\n", recipe.SolidIngredients);
            ingredientsText.text = liquids + solids;
        }

        if (cocktailImage != null && recipe.Image != null)
            cocktailImage.sprite = recipe.Image;

        if (pageIndicatorText != null)
            pageIndicatorText.text = $"{index + 1} / {recipes.Count}";
    }
}