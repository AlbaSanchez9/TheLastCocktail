using System.Collections;
using UnityEngine;

public class WashZone : MonoBehaviour
{
    [SerializeField] private float washDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!RoundManager.Instance.IsRoundActive()) return;

        Glass glass = other.GetComponent<Glass>();
        if (glass == null) return;

        GlassSpawner spawner = glass.GetSpawner();
        spawner?.NotifyGlassBeingWashed(glass);

        StartCoroutine(WashAndDestroy(glass));
    }

    private IEnumerator WashAndDestroy(Glass glass)
    {
        glass.LockGlass();
        glass.Clean();
        Debug.Log("Lavando vaso...");

        yield return new WaitForSeconds(washDuration);

        if (glass != null)
        {
            Debug.Log("Vaso limpio y retirado");
            Destroy(glass.gameObject);
        }
    }
}