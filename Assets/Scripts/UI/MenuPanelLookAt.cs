using UnityEngine;

public class MenuPanelLookAt : MonoBehaviour
{
    private Transform playerCamera;

    void Start()
    {
        // Busca la cámara principal (los ojos del jugador VR)
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (playerCamera == null) return;

        // El panel siempre mira hacia el jugador
        transform.LookAt(playerCamera);

        // Girar 180° para que la cara del panel mire hacia el jugador
        // y no la parte de atrás
        transform.Rotate(0, 180f, 0);
    }
}