using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class KitchenBelt : MonoBehaviour
{
    [SerializeField] private Transform outputPoint;
    [SerializeField] private GameObject olivesPrefab;
    [SerializeField] private GameObject chipsPrefab;
    [SerializeField] private GameObject nachosPrefab;
    [SerializeField] private GameObject peanutsPrefab;
    [SerializeField] private Transform olivesOutputPoint;
    [SerializeField] private Transform chipsOutputPoint;
    [SerializeField] private Transform nachosOutputPoint;
    [SerializeField] private Transform peanutsOutputPoint;

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
            SnackType.Nachos => nachosPrefab,
            SnackType.Peanuts => peanutsPrefab,
            _ => null
        };

        Transform spawnPoint = snack switch
        {
            SnackType.Olives => olivesOutputPoint,
            SnackType.Chips => chipsOutputPoint,
            SnackType.Nachos => nachosOutputPoint,
            SnackType.Peanuts => peanutsOutputPoint,
            _ => outputPoint
        };

        if (prefabToSpawn != null)
        {
            GameObject snackObj = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            snackObj.GetComponent<NetworkObject>().Spawn();
        }

        if (ticketObject != null)
            ticketObject.GetComponent<NetworkObject>().Despawn();
    }
}