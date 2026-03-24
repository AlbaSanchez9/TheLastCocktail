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
        interactionManager.CancelInteractableSelection((IXRSelectInteractable)this);
        enabled = false;

        if (NetworkManager.Singleton.IsServer)
            AttachToGlassClientRpc(glass.NetworkObjectId);
        else
            AttachToGlassRpc(glass.NetworkObjectId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AttachToGlassRpc(ulong glassId)
    {
        AttachToGlassClientRpc(glassId);
    }

    [ClientRpc]
    private void AttachToGlassClientRpc(ulong glassId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects
            .TryGetValue(glassId, out var glassNetObj)) return;

        StartCoroutine(DoAttach(glassNetObj.transform));
    }

    private IEnumerator DoAttach(Transform glassTransform)
    {
        // Desactiva NetworkTransform para que no revierta la posición
        if (TryGetComponent<Unity.Netcode.Components.NetworkTransform>(out var nt))
            nt.enabled = false;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForEndOfFrame();

        transform.SetParent(glassTransform);
    }
}