using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkedSceneObject : NetworkBehaviour
{
    private Rigidbody rb;
    private NetworkObject netObj;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        netObj = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // El servidor controla el objeto al inicio
            netObj.ChangeOwnership(NetworkManager.ServerClientId);

            // Rigidbody activo en servidor
            rb.isKinematic = false;
        }
        else
        {
            // Clientes no mueven el objeto al inicio
            rb.isKinematic = true;
        }
    }
}