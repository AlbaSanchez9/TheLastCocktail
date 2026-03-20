using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TicketSceneSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TicketSpawnEntry
    {
        public GameObject prefab;
        public Transform spawnPoint;
    }

    [SerializeField] private List<TicketSpawnEntry> ticketsToSpawn;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        foreach (var entry in ticketsToSpawn)
        {
            if (entry.prefab == null || entry.spawnPoint == null) continue;

            GameObject ticket = Instantiate(
                entry.prefab,
                entry.spawnPoint.position,
                entry.spawnPoint.rotation
            );
            ticket.GetComponent<NetworkObject>().Spawn();

            Rigidbody rb = ticket.GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
        }
    }
}