using TMPro;
using UnityEngine;

public class StatsBoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI lostCustomersText;

    private void Update()
    {
        if (RoundManager.Instance == null || GameManager.Instance == null) return;

        // Reloj digital MM:SS
        float t = RoundManager.Instance.GetTimeRemaining();
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        if (timeText != null)
            timeText.text = $"{minutes:00}:{seconds:00}";

        // Dinero
        if (moneyText != null)
            moneyText.text = $"${GameManager.Instance.GetMoney()}";

        // Clientes perdidos
        if (lostCustomersText != null)
            lostCustomersText.text = $"Perdidos: {RoundManager.Instance.GetLostCustomers()}";
    }
}