using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BottleSpawnerManager : MonoBehaviour
{
    public static BottleSpawnerManager Instance;

    [System.Serializable]
    public class BottleEntry
    {
        public string bottleName;
        public GameObject prefab;
    }

    [SerializeField] private List<BottleEntry> bottlePrefabs;

    private Dictionary<ulong, string> registeredBottles = new Dictionary<ulong, string>();

    private void Awake() => Instance = this;

    public void RegisterBottle(ulong networkObjectId, string name)
    {
        registeredBottles[networkObjectId] = name;
    }

    public void RequestSpawn(ulong networkObjectId, Vector3 pos, Quaternion rot, float delay)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!registeredBottles.TryGetValue(networkObjectId, out string name)) return;

        GameObject prefab = bottlePrefabs.Find(b => b.bottleName == name)?.prefab;
        if (prefab != null)
            StartCoroutine(SpawnAfterDelay(prefab, pos, rot, delay));
    }

    private IEnumerator SpawnAfterDelay(GameObject prefab, Vector3 pos, Quaternion rot, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newBottle = Instantiate(prefab, pos, rot);
        newBottle.GetComponent<NetworkObject>().Spawn();
        Rigidbody rb = newBottle.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
    }
}