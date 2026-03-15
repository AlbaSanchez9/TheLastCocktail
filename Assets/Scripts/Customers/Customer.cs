using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [SerializeField] private float patience = 30f;
    [SerializeField] private Slider patienceBar;
    [SerializeField] private float stealChance = 0.2f;
    [SerializeField] private GameManager gameManager;

    private float currentPatience;

    private Order order;
    private bool hasDrink = false;

    public void SetOrder(Order newOrder)
    {
        order = newOrder;
        Debug.Log("Cliente pide: " + order.Recipe.CocktailName);
    }

    public void TryServeDrink(string cocktailName, Glass glass)
    {
        if (cocktailName == order.Recipe.CocktailName)
        {
            Debug.Log("Pedido correcto!");

            hasDrink = true;

            glass.MakeDirty();

            LeaveBar();
        }
        else
        {
            Debug.Log("Bebida incorrecta!");
        }
    }

    private void Start()
    {
        currentPatience = patience;

        if (patienceBar != null)
            patienceBar.maxValue = patience;
    }

    private void Update()
    {
        if (hasDrink) return;

        currentPatience -= Time.deltaTime;

        if (patienceBar != null)
            patienceBar.value = currentPatience;

        if (currentPatience <= 0f)
        {
            LeaveBar();
        }
    }

    //public void ReceiveDrink()
    //{
    //    hasDrink = true;

    //    Debug.Log("Cliente recibió su bebida");
    //}

    private void LeaveBar()
    {
        if (hasDrink)
        {
            if (Random.value < stealChance)
            {
                Debug.Log("Cliente intentó irse sin pagar!");
            }
            else
            {
                Debug.Log("Cliente pagó y se fue feliz");

                if (gameManager != null)
                    gameManager.AddMoney(10);
            }
        }
        else
        {
            Debug.Log("Cliente se fue enfadado por esperar demasiado");
        }

        Destroy(gameObject);
    }
}