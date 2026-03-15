using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int money = 0;

    public void AddMoney(int amount)
    {
        money += amount;

        Debug.Log("Dinero actual: " + money);
    }
}