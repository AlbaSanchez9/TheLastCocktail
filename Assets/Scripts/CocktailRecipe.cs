using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Cocktails/Recipe")]
public class CocktailRecipe : ScriptableObject
{
    [SerializeField] private string cocktailName;
    [SerializeField] private List<string> ingredients;

    public string CocktailName => cocktailName;

    public IReadOnlyList<string> Ingredients => ingredients;
}