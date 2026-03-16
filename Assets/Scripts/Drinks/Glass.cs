using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Glass : MonoBehaviour
{
    private List<string> ingredients = new List<string>();

    [SerializeField] private bool isDirty;

    private XRGrabInteractable grab;
    private Rigidbody rb;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

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

    public void LockGlass()
    {
        if (grab != null)
            grab.enabled = false;

        if (rb != null)
            rb.isKinematic = true;
    }

    public void UnlockGlass()
    {
        if (grab != null)
            grab.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }
}