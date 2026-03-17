using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int money = 0;

    private int correctDrinks = 0;
    private int wrongDrinks = 0;

    private int correctSnacks = 0;
    private int wrongSnacks = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    // DRINKS
    public void AddCorrectDrink() => correctDrinks++;
    public void AddWrongDrink() => wrongDrinks++;

    // SNACKS
    public void AddCorrectSnack() => correctSnacks++;
    public void AddWrongSnack() => wrongSnacks++;

    // GETTERS
    public int GetMoney() => money;

    public int GetCorrectDrinks() => correctDrinks;
    public int GetWrongDrinks() => wrongDrinks;

    public int GetCorrectSnacks() => correctSnacks;
    public int GetWrongSnacks() => wrongSnacks;

    public int GetTotalScore()
    {
        return (correctDrinks * 10) +
               (correctSnacks * 8) -
               (wrongDrinks * 5) -
               (wrongSnacks * 3);
    }
}