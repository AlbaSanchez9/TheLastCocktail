using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WashZone : MonoBehaviour
{
    [SerializeField] private float washDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!RoundManager.Instance.IsRoundActive()) return;

        Glass glass = other.GetComponent<Glass>();
        if (glass == null) return;

        glass.GetSpawner()?.NotifyGlassBeingWashed(glass);
        StartCoroutine(WashAndDestroy(glass));
    }

    private IEnumerator WashAndDestroy(Glass glass)
    {
        glass.LockGlass();
        glass.Clean();
        yield return new WaitForSeconds(washDuration);
        if (glass != null)
        {
            glass.DespawnSolidChildren();
            glass.GetComponent<NetworkObject>().Despawn();
        }
    }
}