using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TicketSpawnerManager : MonoBehaviour
{
    public static TicketSpawnerManager Instance;

    [System.Serializable]
    public class TicketEntry
    {
        public string ticketName;
        public GameObject prefab;
    }

    [SerializeField] private List<TicketEntry> ticketPrefabs;

    private Dictionary<ulong, string> registeredTickets = new Dictionary<ulong, string>();

    private void Awake() => Instance = this;

    public void RegisterTicket(ulong networkObjectId, string name, Vector3 pos, Quaternion rot)
    {
        registeredTickets[networkObjectId] = name;
    }

    public void RequestSpawn(ulong networkObjectId, Vector3 pos, Quaternion rot, float delay)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!registeredTickets.TryGetValue(networkObjectId, out string name)) return;

        GameObject prefab = ticketPrefabs.Find(t => t.ticketName == name)?.prefab;
        if (prefab != null)
            StartCoroutine(SpawnAfterDelay(prefab, pos, rot, delay));
    }

    private IEnumerator SpawnAfterDelay(GameObject prefab, Vector3 pos, Quaternion rot, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject copy = Instantiate(prefab, pos, rot);
        copy.GetComponent<NetworkObject>().Spawn();
        Rigidbody rb = copy.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
        var ticket = copy.GetComponent<InfiniteSnackTicket>();
        if (ticket != null)
            ticket.SetOriginalTransform(pos, rot);
    }
}