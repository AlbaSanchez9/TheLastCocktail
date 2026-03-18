using UnityEngine;

public class Bin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Notifica al spawner antes de destruir
        Glass glass = other.GetComponent<Glass>();
        if (glass != null)
        {
            glass.GetSpawner()?.NotifyGlassFell(glass);
            Destroy(other.gameObject);
            return;
        }

        if (other.GetComponent<IngredientBottle>() != null ||
            other.GetComponent<InfiniteSnackTicket>() != null ||
            other.GetComponent<SnackPrefab>() != null)
        {
            Destroy(other.gameObject);
        }
    }
}