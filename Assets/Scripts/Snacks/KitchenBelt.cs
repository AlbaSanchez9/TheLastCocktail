using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class KitchenBelt : MonoBehaviour
{
    [SerializeField] private Transform outputPoint;
    [SerializeField] private GameObject olivesPrefab;
    [SerializeField] private GameObject chipsPrefab;
    [SerializeField] private GameObject tortillaPrefab;
    [SerializeField] private GameObject croquetasPrefab;
    [SerializeField] private Transform olivesOutputPoint;
    [SerializeField] private Transform chipsOutputPoint;
    [SerializeField] private Transform tortillaOutputPoint;
    [SerializeField] private Transform croquetasOutputPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!RoundManager.Instance.IsRoundActive()) return;

        InfiniteSnackTicket ticket = other.GetComponent<InfiniteSnackTicket>();
        if (ticket != null && !ticket.IsBeingProcessed)
        {
            ticket.IsBeingProcessed = true;
            StartCoroutine(ProcessSnack(other.gameObject, ticket.snackType));
        }
    }

    IEnumerator ProcessSnack(GameObject ticketObject, SnackType snack)
    {
        yield return new WaitForSeconds(3f);

        GameObject prefabToSpawn = snack switch
        {
            SnackType.Olives => olivesPrefab,
            SnackType.Chips => chipsPrefab,
            SnackType.Tortilla => tortillaPrefab,
            SnackType.Croquetas => croquetasPrefab,
            _ => null
        };

        Transform spawnPoint = snack switch
        {
            SnackType.Olives => olivesOutputPoint,
            SnackType.Chips => chipsOutputPoint,
            SnackType.Tortilla => tortillaOutputPoint,
            SnackType.Croquetas => croquetasOutputPoint,
            _ => outputPoint
        };

        if (prefabToSpawn != null)
        {
            GameObject snackObj = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            snackObj.GetComponent<NetworkObject>().Spawn();

            Rigidbody rb = snackObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (ticketObject != null)
            ticketObject.GetComponent<NetworkObject>().Despawn();
    }
}