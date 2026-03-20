using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SolidIngredient : XRGrabInteractable
{
    [SerializeField] private string ingredientName;
    public string IngredientName => ingredientName;

    [Header("Caída")]
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;
    private bool falling = false;
    private bool hasBeenPlaced = false;

    private void Update()
    {
        if (falling || hasBeenPlaced) return;
        if (transform.position.y < fallYThreshold)
        {
            falling = true;
            StartCoroutine(WaitThenDestroy());
        }
    }

    private IEnumerator WaitThenDestroy()
    {
        yield return new WaitForSeconds(timeOnFloor);
        if (NetworkManager.Singleton.IsServer)
            GetComponent<NetworkObject>().Despawn();
    }

    public void PlaceInGlass(Glass glass)
    {
        if (hasBeenPlaced) return;
        hasBeenPlaced = true;

        glass.AddSolidIngredient(ingredientName);

        if (TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = true;

        interactionManager.CancelInteractableSelection((IXRSelectInteractable)this);
        enabled = false;

        transform.SetParent(glass.transform);
    }
}