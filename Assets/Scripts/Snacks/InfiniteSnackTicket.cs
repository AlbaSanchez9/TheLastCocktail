using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InfiniteSnackTicket : XRGrabInteractable
{
    [SerializeField] public SnackType snackType;
    [SerializeField] private float respawnDelay = 0.2f;
    [HideInInspector] public bool IsBeingProcessed = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    protected override void Awake()
    {
        base.Awake();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
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