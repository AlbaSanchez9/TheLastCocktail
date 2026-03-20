using Unity.Netcode;
using UnityEngine;

public class Bin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Glass glass = other.GetComponent<Glass>();
        if (glass != null)
        {
            glass.GetSpawner()?.NotifyGlassFell(glass);
            glass.GetComponent<NetworkObject>().Despawn();
            return;
        }

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && (
            other.GetComponent<IngredientBottle>() != null ||
            other.GetComponent<InfiniteSnackTicket>() != null ||
            other.GetComponent<SnackPrefab>() != null ||
            other.GetComponent<SolidIngredient>() != null))
        {
            netObj.Despawn();
        }
    }
}