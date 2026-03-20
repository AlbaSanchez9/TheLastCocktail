using TMPro;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;

    private float dotTimer;
    private int dotCount;

    private void Update()
    {
        // Animación de puntos suspensivos mientras conecta
        dotTimer += Time.deltaTime;
        if (dotTimer > 0.5f)
        {
            dotTimer = 0;
            dotCount = (dotCount + 1) % 4;
            statusText.text = "Conectando" + new string('.', dotCount);
        }
    }
}