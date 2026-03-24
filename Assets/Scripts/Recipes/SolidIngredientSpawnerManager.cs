using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SolidIngredientSpawnerManager : MonoBehaviour
{
    public static SolidIngredientSpawnerManager Instance;

    [System.Serializable]
    public class SolidEntry
    {
        public string ingredientName;
        public GameObject prefab;
    }

    [SerializeField] private List<SolidEntry> solidPrefabs;
    private Dictionary<ulong, string> registeredSolids = new Dictionary<ulong, string>();

    private void Awake() => Instance = this;

    public void RegisterSolid(ulong networkObjectId, string name)
    {
        registeredSolids[networkObjectId] = name;
    }

    public void RequestSpawn(ulong networkObjectId, Vector3 pos, Quaternion rot, float delay)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!registeredSolids.TryGetValue(networkObjectId, out string name)) return;

        GameObject prefab = solidPrefabs.Find(s => s.ingredientName == name)?.prefab;
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
    }
}