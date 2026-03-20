using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IngredientBottle : XRGrabInteractable
{
    [SerializeField] private string ingredientName;
    public string IngredientName => ingredientName;

    [Header("Caída")]
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;

    private bool falling = false;

    private void Update()
    {
        if (falling) return;
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
            GetComponent<NetworkObject>().Despawn(); // ← Despawn en vez de Destroy
    }
}