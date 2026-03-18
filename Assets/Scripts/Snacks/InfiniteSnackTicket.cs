using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InfiniteSnackTicket : XRGrabInteractable
{
    [SerializeField] public SnackType snackType;
    [SerializeField] private float respawnDelay = 0.2f;

    [Header("Caída")]
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;

    [HideInInspector] public bool IsBeingProcessed = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool falling = false;

    protected override void Awake()
    {
        base.Awake();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (falling) return;
        if (transform.position.y < fallYThreshold)
        {
            falling = true;
            StartCoroutine(WaitThenDestroy());
        }
    }

    private IEnumerator WaitThenDestroy()
    {
        yield return new WaitForSeconds(timeOnFloor);
        Destroy(gameObject);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        StartCoroutine(SpawnCopy());
    }

    private IEnumerator SpawnCopy()
    {
        yield return new WaitForSeconds(respawnDelay);
        GameObject copy = Instantiate(gameObject, originalPosition, originalRotation);

        Rigidbody rb = copy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        var copyInteractable = copy.GetComponent<InfiniteSnackTicket>();
        if (copyInteractable != null)
            copyInteractable.SetOriginalTransform(originalPosition, originalRotation);
    }

    public void SetOriginalTransform(Vector3 pos, Quaternion rot)
    {
        originalPosition = pos;
        originalRotation = rot;
    }
}