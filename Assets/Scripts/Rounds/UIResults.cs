using TMPro;
using UnityEngine;

public class UIResults : MonoBehaviour
{
    public static UIResults Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultsText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);

        resultsText.text =
            $"DINERO: {GameManager.Instance.GetMoney()}\n\n" +

            $"BEBIDAS:\n" +
            $"Correctas: {GameManager.Instance.GetCorrectDrinks()}\n" +
            $"Incorrectas: {GameManager.Instance.GetWrongDrinks()}\n\n" +

            $"SNACKS:\n" +
            $"Correctos: {GameManager.Instance.GetCorrectSnacks()}\n" +
            $"Incorrectos: {GameManager.Instance.GetWrongSnacks()}" +
            $"\nSCORE TOTAL: {GameManager.Instance.GetTotalScore()}";
    }
}