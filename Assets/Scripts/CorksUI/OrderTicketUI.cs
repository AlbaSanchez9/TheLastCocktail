using TMPro;
using UnityEngine;

public class OrderTicketUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cocktailText;
    [SerializeField] private TextMeshProUGUI snackText;

    public void Setup(string cocktailName, string snackName, bool wantsDrink, bool wantsSnack)
    {
        if (cocktailText != null)
            cocktailText.text = wantsDrink ? $"{cocktailName}" : "-";

        if (snackText != null)
            snackText.text = wantsSnack ? $"{snackName}" : "-";
    }
}