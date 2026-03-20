using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TicketSpawner : NetworkBehaviour
{
    [SerializeField] private float respawnDelay = 0.2f;
    [SerializeField] private string ticketName;

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
        TicketSpawnerManager.Instance?.RegisterTicket(NetworkObjectId, ticketName, spawnPos, spawnRot);
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
        TicketSpawnerManager.Instance?.RequestSpawn(NetworkObjectId, pos, rot, respawnDelay);
    }
}