using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PricingConfig pricing;

    private int money = 0;
    private int correctDrinks = 0;
    private int wrongDrinks = 0;
    private int correctSnacks = 0;
    private int wrongSnacks = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyLostCustomerPenalty(bool wantedDrink, bool wantedSnack, bool drinkServed, bool snackServed)
    {
        int penalty = 0;

        if (wantedDrink && wantedSnack)
        {
            if (drinkServed && !snackServed)
                penalty = pricing.partialDrinkServedPenalty;
            else if (!drinkServed && snackServed)
                penalty = pricing.partialSnackServedPenalty;
            else if (!drinkServed && !snackServed)
                penalty = pricing.lostBothPenalty;
            // drinkServed && snackServed → penalty = 0
        }
        else if (wantedDrink)
            penalty = drinkServed ? 0 : pricing.lostDrinkOnlyPenalty;
        else if (wantedSnack)
            penalty = snackServed ? 0 : pricing.lostSnackOnlyPenalty;

        money = Mathf.Max(0, money - penalty);
        Debug.Log($"Penalización: -${penalty} | Total: ${money}");
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Dinero añadido: +${amount} | Total: ${money}");
    }

    public void AddCorrectDrink() => correctDrinks++;
    public void AddWrongDrink() => wrongDrinks++;
    public void AddCorrectSnack() => correctSnacks++;
    public void AddWrongSnack() => wrongSnacks++;

    public int GetMoney() => money;
    public int GetCorrectDrinks() => correctDrinks;
    public int GetWrongDrinks() => wrongDrinks;
    public int GetCorrectSnacks() => correctSnacks;
    public int GetWrongSnacks() => wrongSnacks;
    public int GetTotalScore() => money;
    public PricingConfig GetPricing() => pricing;
}