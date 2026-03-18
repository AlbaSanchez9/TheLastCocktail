using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BottleSpawner : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 3f;

    private XRGrabInteractable grab;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private bool hasSpawned = false; // ← solo spawnea una vez

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
    }

    private void Start()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    private void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (hasSpawned) return; // ya spawneó, ignorar
        hasSpawned = true;
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject newBottle = Instantiate(gameObject, spawnPos, spawnRot);

        Rigidbody rb = newBottle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}