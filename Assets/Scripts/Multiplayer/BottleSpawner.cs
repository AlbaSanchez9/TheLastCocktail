using Unity.Netcode;
using UnityEngine;

public class BottleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bottlePrefab;

    public void SpawnBottle(Vector3 position, Quaternion rotation)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GameObject obj = Instantiate(bottlePrefab, position, rotation);
        obj.GetComponent<NetworkObject>().Spawn();
    }
}