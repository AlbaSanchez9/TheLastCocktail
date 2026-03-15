using UnityEngine;

public class WashZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Glass glass = other.GetComponent<Glass>();

        if (glass == null) return;

        glass.Clean();

        Debug.Log("Vaso lavado");
    }
}