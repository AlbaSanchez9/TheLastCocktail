using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int totalMoney = 0;
    private int satisfiedCustomers = 0;
    private int angryCustomers = 0;

    public int TotalMoney => totalMoney;
    public int SatisfiedCustomers => satisfiedCustomers;
    public int AngryCustomers => angryCustomers;
    public int TotalCustomers => satisfiedCustomers + angryCustomers;

    // Umbrales configurables en el Inspector
    [Header("Umbrales de estrellas")]
    [SerializeField] private int moneyFor3Stars = 200;
    [SerializeField] private int moneyFor2Stars = 100;
    [SerializeField] private float satisfactionFor3Stars = 0.8f; // 80% clientes satisfechos
    [SerializeField] private float satisfactionFor2Stars = 0.5f; // 50% clientes satisfechos

    public void AddMoney(int amount) { totalMoney += amount; }
    public void RegisterSatisfied() { satisfiedCustomers++; }
    public void RegisterAngry() { angryCustomers++; }

    public float GetSatisfactionRate()
    {
        if (TotalCustomers == 0) return 1f;
        return (float)satisfiedCustomers / TotalCustomers;
    }

    public int CalculateStars()
    {
        bool moneyOk3 = totalMoney >= moneyFor3Stars;
        bool moneyOk2 = totalMoney >= moneyFor2Stars;
        bool satisfyOk3 = GetSatisfactionRate() >= satisfactionFor3Stars;
        bool satisfyOk2 = GetSatisfactionRate() >= satisfactionFor2Stars;

        if (moneyOk3 && satisfyOk3) return 3;
        if (moneyOk2 && satisfyOk2) return 2;
        return 1;
    }
}