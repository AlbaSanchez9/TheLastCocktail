using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject resultsPanel;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI satisfiedText;
    [SerializeField] private TextMeshProUGUI angryText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI titleText;

    private ScoreManager scoreManager;

    private void Awake()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        resultsPanel.SetActive(false);
    }

    public void ShowResults(bool wasGameOver)
    {
        resultsPanel.SetActive(true);

        int stars = scoreManager.CalculateStars();

        titleText.text = wasGameOver ? "BAR COLAPSADO" : "TURNO COMPLETADO";
        moneyText.text = $"Dinero: {scoreManager.TotalMoney}€";
        satisfiedText.text = $"Clientes satisfechos: {scoreManager.SatisfiedCustomers}";
        angryText.text = $"Clientes enfadados: {scoreManager.AngryCustomers}";
        starsText.text = new string('★', stars) + new string('☆', 3 - stars);
    }
}