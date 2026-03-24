using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SolidSpawner : NetworkBehaviour
{
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private string ingredientName;

    private XRGrabInteractable grab;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private bool hasSpawned = false;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
    }

    public override void OnNetworkSpawn()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
        SolidIngredientSpawnerManager.Instance?.RegisterSolid(NetworkObjectId, ingredientName);
    }

    private void OnDestroy()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (hasSpawned) return;
        hasSpawned = true;
        NotifyGrabbedRpc(spawnPos, spawnRot);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void NotifyGrabbedRpc(Vector3 pos, Quaternion rot)
    {
        SolidIngredientSpawnerManager.Instance?.RequestSpawn(NetworkObjectId, pos, rot, respawnDelay);
    }
}