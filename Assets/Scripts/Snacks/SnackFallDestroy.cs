using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SnackFallDestroy : MonoBehaviour
{
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;

    private bool falling = false;

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
}