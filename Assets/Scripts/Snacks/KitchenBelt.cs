using System.Collections;
using UnityEngine;

public class KitchenBelt : MonoBehaviour
{
    [SerializeField] private Transform outputPoint;

    [Header("Snack Prefabs")]
    [SerializeField] private GameObject olivesPrefab;
    [SerializeField] private GameObject chipsPrefab;
    [SerializeField] private GameObject nachosPrefab;
    [SerializeField] private GameObject peanutsPrefab;

    [Header("Snack Output Points")]
    [SerializeField] private Transform olivesOutputPoint;
    [SerializeField] private Transform chipsOutputPoint;
    [SerializeField] private Transform nachosOutputPoint;
    [SerializeField] private Transform peanutsOutputPoint;

    private void OnTriggerEnter(Collider other)
    {
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
            Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

        Destroy(ticketObject);
    }
}