using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Cocktails/Recipe")]
public class CocktailRecipe : ScriptableObject
{
    [SerializeField] private string cocktailName;
    [SerializeField] private List<string> liquidIngredients; 
    [SerializeField] private List<string> solidIngredients;  
    [SerializeField] private Sprite image;
    [SerializeField] private Color drinkColor = Color.white;

    public string CocktailName => cocktailName;

    public IReadOnlyList<string> LiquidIngredients => liquidIngredients;
    public IReadOnlyList<string> SolidIngredients => solidIngredients;

    public Sprite Image => image;

    public Color DrinkColor => drinkColor;
}