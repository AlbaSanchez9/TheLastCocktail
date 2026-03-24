using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SolidSceneSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SolidSpawnEntry
    {
        public GameObject prefab;
        public Transform spawnPoint;
    }

    [SerializeField] private List<SolidSpawnEntry> solidsToSpawn;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        foreach (var entry in solidsToSpawn)
        {
            if (entry.prefab == null || entry.spawnPoint == null) continue;

            GameObject solid = Instantiate(
                entry.prefab,
                entry.spawnPoint.position,
                entry.spawnPoint.rotation
            );
            solid.GetComponent<NetworkObject>().Spawn();

            Rigidbody rb = solid.GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
        }
    }
}