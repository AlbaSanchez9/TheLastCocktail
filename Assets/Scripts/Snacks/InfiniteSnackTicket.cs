using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InfiniteSnackTicket : MonoBehaviour
{
    [SerializeField] public SnackType snackType;

    [Header("Caída")]
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;

    [HideInInspector] public bool IsBeingProcessed = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool falling = false;

    private XRGrabInteractable grab;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.retainTransformParent = false;
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.throwOnDetach = false;
        }
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
        if (NetworkManager.Singleton.IsServer)
            GetComponent<NetworkObject>().Despawn();
    }

    public void SetOriginalTransform(Vector3 pos, Quaternion rot)
    {
        originalPosition = pos;
        originalRotation = rot;
    }
}