using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Cocktails/Recipe")]
public class CocktailRecipe : ScriptableObject
{
    [SerializeField] private string cocktailName;
    [SerializeField] private List<string> ingredients;
    [SerializeField] private Sprite image;

    public string CocktailName => cocktailName;

    public IReadOnlyList<string> Ingredients => ingredients;

    public Sprite Image => image;
}