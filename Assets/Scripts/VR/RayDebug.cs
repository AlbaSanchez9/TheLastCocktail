// Assets/_Game/Scripts/Debug/RayDebug.cs
using UnityEngine;
using Unity.XR.CoreUtils;

public class RayDebug : MonoBehaviour
{
    void Update()
    {
        // Dibuja el rayo en la Scene View para ver a dónde apunta
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);

        // Raycast para ver qué golpea
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10f))
        {
            Debug.Log($"🎯 Rayo golpea: {hit.collider.gameObject.name}");
        }
        else
        {
            Debug.Log("❌ Rayo no golpea nada");
        }
    }
}