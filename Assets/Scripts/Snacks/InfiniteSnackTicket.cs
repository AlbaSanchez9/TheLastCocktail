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

    [Header("Pegado en pared")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float stickDistance = 0.15f;
    [SerializeField] private float stickOffset = 0.01f;


    [HideInInspector] public bool IsBeingProcessed = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool falling = false;
    private bool isStuck = false;

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
            grab.selectExited.AddListener(OnSelectExited);
        }
    }

    private void Update()
    {
        if (isStuck || falling) return;
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

    private void OnDestroy()
    {
        if (grab != null)
            grab.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (!isStuck)
            TryStickToWall();
    }

    private void TryStickToWall()
    {
        Vector3[] directions = {
            transform.forward, -transform.forward,
            transform.right,   -transform.right,
            transform.up,      -transform.up
        };

        float closestDist = stickDistance;
        RaycastHit bestHit = default;
        bool found = false;

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, closestDist))
            {
                if (hit.collider.CompareTag(wallTag))
                {
                    closestDist = hit.distance;
                    bestHit = hit;
                    found = true;
                }
            }
        }

        if (found)
            StickToWallRpc(bestHit.point, bestHit.normal);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void StickToWallRpc(Vector3 hitPoint, Vector3 hitNormal)
    {
        StickClientRpc(hitPoint, hitNormal);
    }

    [ClientRpc]
    private void StickClientRpc(Vector3 hitPoint, Vector3 hitNormal)
    {
        isStuck = true;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = hitPoint + hitNormal * stickOffset;
        transform.rotation = Quaternion.LookRotation(-hitNormal);
    }
}