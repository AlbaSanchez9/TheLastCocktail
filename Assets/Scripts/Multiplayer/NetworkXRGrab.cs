using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(XRGrabInteractable))]
public class NetworkXRGrab : NetworkBehaviour
{
    private XRGrabInteractable grab;
    private NetworkObject netObj;

    private bool isHeld = false;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        netObj = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;

        if (!IsOwner)
        {
            RequestOwnershipRpc();
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;

        if (IsOwner)
        {
            ReturnOwnershipRpc();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void RequestOwnershipRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!isHeld)
        {
            netObj.ChangeOwnership(clientId);
        }
    }

    [Rpc(SendTo.Server)]
    void ReturnOwnershipRpc()
    {
        netObj.ChangeOwnership(NetworkManager.ServerClientId);
    }
}