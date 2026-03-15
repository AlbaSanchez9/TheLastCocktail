using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IngredientBottle : XRGrabInteractable
{
    [SerializeField] private string ingredientName;

    public string IngredientName => ingredientName;
}